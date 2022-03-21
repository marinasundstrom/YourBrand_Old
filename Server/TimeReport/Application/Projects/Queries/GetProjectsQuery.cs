﻿
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourCompany.TimeReport.Application.Common.Interfaces;
using YourCompany.TimeReport.Application.Common.Models;

namespace YourCompany.TimeReport.Application.Projects.Queries;

public class GetProjectsQuery : IRequest<ItemsResult<ProjectDto>>
{
    public GetProjectsQuery(int page = 0, int pageSize = 10, string? userId = null, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null)
    {
        Page = page;
        PageSize = pageSize;
        UserId = userId;
        SearchString = searchString;
        SortBy = sortBy;
        SortDirection = sortDirection;
    }

    public int Page { get; }

    public int PageSize { get; }

    public string? UserId { get; }

    public string? ProjectId { get; }

    public string? SearchString { get; }

    public string? SortBy { get; }

    public Application.Common.Models.SortDirection? SortDirection { get; }

    public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, ItemsResult<ProjectDto>>
    {
        private readonly ITimeReportContext _context;

        public GetProjectsQueryHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ItemsResult<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Projects
                .Include(p => p.Memberships)
                .OrderBy(p => p.Created)
                .AsNoTracking()
                .AsSplitQuery();

            if (request.UserId is not null)
            {
                query = query.Where(project => project.Memberships.Any(pm => pm.User.Id == request.UserId));
            }

            if (request.SearchString is not null)
            {
                query = query.Where(project => project.Name.ToLower().Contains(request.SearchString.ToLower()) || project.Description.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? TimeReport.Application.SortDirection.Descending : TimeReport.Application.SortDirection.Ascending);
            }

            var projects = await query
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize)
                .ToListAsync();

            var dtos = projects.Select(project => new ProjectDto(project.Id, project.Name, project.Description));

            return new ItemsResult<ProjectDto>(dtos, totalItems);
        }
    }
}