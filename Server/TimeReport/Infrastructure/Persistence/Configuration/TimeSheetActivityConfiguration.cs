﻿
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourCompany.TimeReport.Domain.Entities;

namespace YourCompany.TimeReport.Infrastructure.Persistence.Configurations;

public class TimeSheetActivityConfiguration : IEntityTypeConfiguration<TimeSheetActivity>
{
    public void Configure(EntityTypeBuilder<TimeSheetActivity> builder)
    {
        builder.ToTable("TimeSheetActivities", t => t.IsTemporal());
        builder.HasQueryFilter(i => i.Deleted == null);

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.DeletedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);
    }
}