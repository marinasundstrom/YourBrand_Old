using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using YourBrand.IdentityManagement.Application.Users;
using YourBrand.IdentityManagement.Application.Organizations;
using YourBrand.IdentityManagement.Domain.Entities;

namespace YourBrand.IdentityManagement.Application;

public static class Mapper
{
    public static OrganizationDto ToDto(this Organization organization) => new OrganizationDto(organization.Id, organization.Name, organization.FriendlyName);

    public static UserDto ToDto(this User user) => new UserDto(user.Id, user.Organization?.ToDto(), user.FirstName, user.LastName, user.DisplayName, user.Email,
                    user.Created, user.LastModified);

    /* public static User2Dto ToDto2(this User user) => new User2Dto(user.Id, user.Organization.ToDto(), user.FirstName, user.LastName, user.DisplayName, user.Title,
                user.Department == null ? null : new DepartmentDto(user.Department.Id, user.Department.Name)); */
}