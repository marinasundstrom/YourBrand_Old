﻿namespace YourBrand.HumanResources.Domain.Common.Interfaces;

public interface ISoftDelete
{
    DateTime? Deleted { get; set; }

    string? DeletedBy { get; set; }
}