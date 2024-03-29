
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Sales.Domain.Entities;

namespace YourBrand.Sales.API.Persistence.Configurations;

public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.Ignore(e => e.DomainEvents);

        builder.HasQueryFilter(e => e.Deleted == null);
    }
}