﻿
using YourBrand.TimeReport.Domain.Common;
using YourBrand.TimeReport.Domain.Common.Interfaces;

namespace YourBrand.TimeReport.Domain.Entities;

public class ProjectMembership : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public Project Project { get; set; } = null!;

    public User User { get; set; } = null!;

    public DateTime? From { get; set; }

    public DateTime? To { get; set; }

    /// <summary>
    /// Expected hours per week / timesheet
    /// </summary>
    public double? ExpectedHoursWeekly { get; set; }

    /// <summary>
    /// Required hours per week / timesheet
    /// </summary>
    public double? RequiredHoursWeekly { get; set; }

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}