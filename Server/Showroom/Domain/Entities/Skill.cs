﻿using YourCompany.Showroom.Domain.Common;

namespace YourCompany.Showroom.Domain.Entities;

public class Skill : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public SkillArea Area { get; set; } = null!;

    public string Name { get; set; }  = null!;
    public string? Description { get; set; }

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }

    public List<ConsultantProfile> ConsultantProfiles { get; set; } = new List<ConsultantProfile>();

    public List<ConsultantProfileSkill> ConsultantProfileSkills { get; set; } = new List<ConsultantProfileSkill>();
}
