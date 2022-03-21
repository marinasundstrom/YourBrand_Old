﻿using YourCompany.Showroom.Domain.Common;

namespace YourCompany.Showroom.Domain.Entities;

public class CompetenceArea : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;
    public string Name { get; set; }  = null!;

    public CompetenceArea? Parent { get; set; }

    public ICollection<CompetenceArea> Children { get; set; } = new List<CompetenceArea>();

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}