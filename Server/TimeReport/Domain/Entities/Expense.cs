﻿
using YourCompany.TimeReport.Domain.Common;
using YourCompany.TimeReport.Domain.Common.Interfaces;

namespace YourCompany.TimeReport.Domain.Entities;

public class Expense : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public Project Project { get; set; } = null!;

    public ExpenseType Type { get; set; }

    public DateOnly Date { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public string? Attachment { get; set; }

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}