﻿
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourCompany.TimeReport.Domain.Entities;

namespace YourCompany.TimeReport.Infrastructure.Persistence.Configurations;

public class MonthEntryGroupConfiguration : IEntityTypeConfiguration<MonthEntryGroup>
{
    public void Configure(EntityTypeBuilder<MonthEntryGroup> builder)
    {
        builder.ToTable("MonthEntryGroups", t => t.IsTemporal());

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);
    }
}