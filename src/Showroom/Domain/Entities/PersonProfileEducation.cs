namespace YourBrand.Showroom.Domain.Entities;

using YourBrand.Showroom.Domain.Common;

public class PersonProfileEducation : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public PersonProfile PersonProfile { get; set; } = null!;

    public string School { get; set; } = null!;
    public string Degree { get; set; } = null!;
    public string? FieldOfStudy { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? ExpectedEndDate { get; set; }

    public string Grade { get; set; }
    public string Activities { get; set; }

    public string? Description { get; set; }

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}
