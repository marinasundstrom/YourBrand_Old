﻿using YourBrand.Domain.Entities;

namespace YourBrand.Domain.Common;

public abstract class AuditableEntity: Entity
{
    public DateTime Created { get; set; }

    public string? CreatedById { get; set; }

    public User? CreatedBy { get; set; }

    public DateTime? LastModified { get; set; }

    public string? LastModifiedById { get; set; }

    public User? LastModifiedBy { get; set; }
}
