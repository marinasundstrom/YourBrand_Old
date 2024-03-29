
using YourBrand.Showroom.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Showroom.Infrastructure.Persistence.Configurations;

class PersonProfileExperienceSkillConfiguration : IEntityTypeConfiguration<PersonProfileExperienceSkill>
{
    public void Configure(EntityTypeBuilder<PersonProfileExperienceSkill> builder)
    {
        builder.ToTable("PersonProfileExperienceSkills");
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.HasQueryFilter(i => i.Deleted == null);

        builder.HasOne(x => x.PersonProfileExperience)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.PersonProfileSkill)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

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
