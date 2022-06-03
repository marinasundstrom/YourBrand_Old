// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

namespace YourBrand.IdentityService.Domain.Entities;

public class Organization 
{
    private readonly HashSet<Team> _teams = new HashSet<Team>();
    private readonly HashSet<User> _users = new HashSet<User>();

    private Organization() { }

    public Organization(string name)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
    }

    public string Id { get; private set; }

    public string Name { get; private set; }

    public IReadOnlyCollection<Team> Teams => _teams;

    public IReadOnlyCollection<User> Users => _users;
}
