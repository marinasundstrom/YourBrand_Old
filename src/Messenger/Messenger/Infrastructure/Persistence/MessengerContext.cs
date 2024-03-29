﻿
using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Domain.Common;
using YourBrand.Messenger.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using YourBrand.Messenger.Infrastructure.Persistence.Interceptors;
using Newtonsoft.Json;
using YourBrand.Messenger.Infrastructure.Persistence.Outbox;

namespace YourBrand.Messenger.Infrastructure.Persistence;

public class MessengerContext : DbContext, IMessengerContext
{
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public MessengerContext(
        DbContextOptions<MessengerContext> options,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) : base(options)
    {
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);

#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging(); 
#endif
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Configurations.MessageConfiguration).Assembly);
    }

#nullable disable

    public DbSet<Conversation> Conversations { get; set; } = null!;

    public DbSet<ConversationParticipant> ConversationParticipants { get; set; } = null!;

    public DbSet<Message> Messages { get; set; } = null!;

    public DbSet<MessageReceipt> MessageReceipts { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;

#nullable restore

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker
                        .Entries<Entity>()
                        .Where(e => e.Entity.DomainEvents.Any())
                        .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .OrderBy(e => e.Timestamp)
            .ToList();

        var outboxMessages = domainEvents.Select(domainEvent =>
        {
            return new OutboxMessage()
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = DateTime.UtcNow,
                Type = domainEvent.GetType().Name,
                Content = JsonConvert.SerializeObject(
                    domainEvent,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    })
            };
        }).ToList();

        this.Set<OutboxMessage>().AddRange(outboxMessages);

        return await base.SaveChangesAsync(cancellationToken);
    }
}