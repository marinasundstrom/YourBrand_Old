﻿
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Application.Common.Models;
using YourBrand.IdentityManagement.Domain.Entities;

namespace YourBrand.IdentityManagement.Application.Organizations.Queries;

public record GetOrganizationsQuery(int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, IdentityManagement.Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<OrganizationDto>>
{
    public class GetOrganizationsQueryHandler : IRequestHandler<GetOrganizationsQuery, ItemsResult<OrganizationDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetOrganizationsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ItemsResult<OrganizationDto>> Handle(GetOrganizationsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Organizations
                .OrderBy(p => p.Created)
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize)
                .AsNoTracking()
                .AsSplitQuery();

            if (request.SearchString is not null)
            {
                query = query.Where(p =>
                    p.Name.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == IdentityManagement.Application.Common.Models.SortDirection.Desc ? IdentityManagement.Application.SortDirection.Descending : IdentityManagement.Application.SortDirection.Ascending);
            }

            var organizations = await query
                .ToListAsync(cancellationToken);

            var dtos = organizations.Select(organization => organization.ToDto());

            return new ItemsResult<OrganizationDto>(dtos, totalItems);
        }
    }
}