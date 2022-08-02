using YourBrand.Showroom.Domain.Entities;
using YourBrand.Showroom.Domain.ValueObjects;
using YourBrand.Showroom.Infrastructure.Persistence;

using Microsoft.Extensions.DependencyInjection;
using YourBrand.Showroom.TestData;
using Microsoft.EntityFrameworkCore;

namespace YourBrand.Showroom.Infrastructure.Persistence;

public static class Seed
{
    public static async Task SeedAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ShowroomContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        if (!context.Languages.Any())
        {
            var language = new Language
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Swedish",
                NativeName = "svenska",
                ISO639 = "sv"
            };

            context.Languages.Add(language);

            language = new Language
            {
                Id = Guid.NewGuid().ToString(),
                Name = "English",
                NativeName = "English",
                ISO639 = "en"
            };

            context.Languages.Add(language);
        }

        if (!context.Users.Any())
        {
            context.Users.Add(new User
            {
                Id = "api",
                FirstName = "API",
                LastName = "User",
                SSN = "213",
                Email = "test@foo.com",
                Hidden = true
            });

            await context.SaveChangesAsync();
        }

        if (!context.Organizations.Any())
        {
            context.Organizations.Add(new Organization
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Organization",
                Address = new Address
                {
                    Address1 = "",
                    Address2 = "",
                    PostalCode = "",
                    Locality = "",
                    SubAdminArea = "",
                    AdminArea = "",
                    Country = ""
                },
            });

            await context.SaveChangesAsync();
        }

        if (!context.CompetenceAreas.Any())
        {
            context.CompetenceAreas.Add(new CompetenceArea
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Area 1"
            });

            context.CompetenceAreas.Add(new CompetenceArea
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Area 2"
            });

            context.CompetenceAreas.Add(new CompetenceArea
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Area 3"
            });

            await context.SaveChangesAsync();
        }

        /*

        if (!context.SkillAreas.Any())
        {
            var area1 = new SkillArea
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Software Architecture",
                Slug = "software-architecture"
            };

            area1.Skills.Add(new Skill
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Microservices",
                Slug = "microservices"
            });

            area1.Skills.Add(new Skill
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Event-driven Architecture",
                Slug = "event-driven-architecture"
            });

            context.SkillAreas.Add(area1);

            var area2 = new SkillArea
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Development Platforms",
                Slug = "development-platforms"
            };

            area2.Skills.Add(new Skill
            {
                Id = Guid.NewGuid().ToString(),
                Name = ".NET",
                Slug = "net"
            });

            area2.Skills.Add(new Skill
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Java",
                Slug = "java"
            });

            area2.Skills.Add(new Skill
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Node",
                Slug = "node"
            });

            area2.Skills.Add(new Skill
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Web",
                Slug = "web"
            });

            context.SkillAreas.Add(area2);

            await context.SaveChangesAsync();
        }
        */

        ConsultantProfile consultantProfile = new ConsultantProfile()
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = "Marina",
            LastName = "Sundström",
            DisplayName = null,
            BirthDate = new DateTime(1990, 1, 5),
            Organization = await context.Organizations.FirstAsync(),
            CompetenceArea = await context.CompetenceAreas.FirstAsync(),
            Headline = "Senior Software Developer",
            ShortPresentation = "I'm as Software developer who is based in Malmö, Sweden.",
            Presentation = @"
I'm as Software developer who is based in Malmö, Sweden.

I have been programming since 2007. My interest back then was to figure out how it all works. Since then I have been learning a lot about software engineering, both professionally and in my free time.

My career began back in 2014, when I was working as a software developer for a local company that provided Internet and IT services. After that I went into consulting, where I got to experience software development at various companies and in many fields. I have maninly been working with technologies such as .NET and Web.",
        };

        context.ConsultantProfiles.Add(consultantProfile);

        await LoadTestData(context, consultantProfile);

        await context.SaveChangesAsync();

        /*
        consultantProfile.Languages.Add(new LanguageSkill {
            Language = context.Languages.FirstOrDefault(l => l.ISO639 == "sv"),
            SkillLevel = SkillLevel.Native
        });
        */
    }

    private static async Task LoadTestData(ShowroomContext context, ConsultantProfile consultantProfile)
    {
        var skillGroups = Skills2.FromJson(await File.ReadAllTextAsync("../TestData/skills.json"));
        foreach (var skillGroup in skillGroups)
        {
            var skillArea = new Domain.Entities.SkillArea()
            {
                Id = Guid.NewGuid().ToString(),
                Name = skillGroup.Key,
                Slug = NewMethod(skillGroup.Key),
            };

            foreach (var skillPair in skillGroup.Value)
            {
                var skillName = skillPair.Key;
                var skillInfo = skillPair.Value;

                var skill = new Domain.Entities.Skill()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = skillName,
                    Slug = NewMethod(skillName),
                };

                skillArea.Skills.Add(skill);
                
                consultantProfile.ConsultantProfileSkills.Add(new ConsultantProfileSkill
                {
                    Id = Guid.NewGuid().ToString(),
                    Skill = skill,
                    //Years = skillInfo.Years,
                    Level = (Domain.Enums.SkillLevel?)skillInfo.Level,
                    Comment = skillInfo.Comment,
                    Link = skillInfo.Link is null ? null : new Domain.ValueObjects.Link(skillInfo.Link.Title, skillInfo.Link.Href)
                });
            }

            context.SkillAreas.Add(skillArea);

            await context.SaveChangesAsync();
        }

        var resume = Resume.FromJson(await File.ReadAllTextAsync("../TestData/resume.json"));
        foreach (var experience in resume.Experience)
        {
            var experience2 = new Domain.Entities.ConsultantProfileExperience()
            {
                Id = Guid.NewGuid().ToString(),
                ConsultantProfile = consultantProfile,
                Current = experience.Current,
                Highlight = experience.Highlight,
                CompanyName = experience.Company,
                CompanyLogo = experience.CompanyLogo,
                Link = experience.Link,
                Location = experience.Location,
                Title = experience.Title,
                EmploymentType = experience.EmploymentType,
                StartDate = experience.StartDate,
                EndDate = experience.EndDate,
                Description = experience.Description
            };

            foreach (var skill in experience.Skills)
            {
                var name = NewMethod(skill);

                var sk = await context.ConsultantProfileSkills.FirstOrDefaultAsync(x => x.Skill.Slug == name);

                if (sk is null)
                {
                    var sk2 = await context.Skills.FirstOrDefaultAsync(x => x.Slug == name);

                    if (sk2 is null) continue;

                    sk = new ConsultantProfileSkill()
                    {
                        Id = Guid.NewGuid().ToString(),
                        ConsultantProfile = consultantProfile,
                        Skill = sk2!
                    };

                    consultantProfile.ConsultantProfileSkills.Add(sk);
                }

                experience2.Skills.Add(new ConsultantProfileExperienceSkill()
                {
                    Id = Guid.NewGuid().ToString(),
                    ConsultantProfileExperience = experience2,
                    ConsultantProfileSkill = sk
                });
            }

            consultantProfile.Experience.Add(experience2);
        }

        await context.SaveChangesAsync();
    }

    private static string NewMethod(string skillName)
    {
        return skillName
                            .ToLower()
                            .Replace(" ", "-")
                            .Replace("(", string.Empty)
                            .Replace(")", string.Empty)
                            .Replace(".", string.Empty)
                            .Replace("#", string.Empty);
    }
}
