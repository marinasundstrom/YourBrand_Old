﻿using Microsoft.EntityFrameworkCore;

using Skynet.IdentityService.Domain.Entities;

namespace Skynet.IdentityService.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<ApplicationUser> Users { get; }
    DbSet<Team> Teams { get; }
    DbSet<Department> Departments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}