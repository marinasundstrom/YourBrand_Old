﻿
using YourBrand.Accounting.Application.Common.Models;
using YourBrand.Accounting.Domain.Events;

using Microsoft.Extensions.Logging;
using YourBrand.Accounting.Application.Common.Interfaces;

namespace YourBrand.Accounting.Application.EventHandlers;

public class EntryCreatedEventHandler : IDomainEventHandler<EntryCreatedEvent>
{
    private readonly ILogger<EntryCreatedEventHandler> _logger;

    public EntryCreatedEventHandler(ILogger<EntryCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EntryCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entry created: {notification.EntryId}");

        return Task.CompletedTask;
    }
}