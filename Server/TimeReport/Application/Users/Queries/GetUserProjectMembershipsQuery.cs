﻿
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourCompany.TimeReport.Application.Common.Interfaces;
using YourCompany.TimeReport.Application.Common.Models;
using YourCompany.TimeReport.Application.Projects;
using YourCompany.TimeReport.Application.Users;
using YourCompany.TimeReport.Domain.Exceptions;

namespace YourCompany.TimeReport.Application.Users.Queries;

public class GetUserProjectMembershipsQuery : IRequest<ItemsResult<ProjectMembershipDto>>
{
    public GetUserProjectMembershipsQuery(string userId, int page = 0, int pageSize = 10, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null)
    {
        UserId = userId;
        Page = page;
        PageSize = pageSize;
        SortBy = sortBy;
        SortDirection = sortDirection;
    }

    public string UserId { get; }

    public int Page { get; }

    public int PageSize { get; }

    public string? SearchString { get; }

    public string? SortBy { get; }

    public Application.Common.Models.SortDirection? SortDirection { get; }

    public class GetUserProjectMembershipsQueryHandler : IRequestHandler<GetUserProjectMembershipsQuery, ItemsResult<ProjectMembershipDto>>
    {
        private readonly ITimeReportContext _context;

        public GetUserProjectMembershipsQueryHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ItemsResult<ProjectMembershipDto>> Handle(GetUserProjectMembershipsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.ProjectMemberships
                    .OrderBy(p => p.Created)
                    .Where(x => x.User.Id == request.UserId);

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? TimeReport.Application.SortDirection.Descending : TimeReport.Application.SortDirection.Ascending);
            }

            var projectMemberships = await query
                .Include(m => m.Project)
                .Include(m => m.User)
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize)
                .AsSplitQuery()
                .ToArrayAsync(cancellationToken);

            var dtos = projectMemberships
                .DistinctBy(x => x.Project) // Temp
                .Select(m => new ProjectMembershipDto(m.Id, new ProjectDto(m.Project.Id, m.Project.Name, m.Project.Description),
                new UserDto(m.User.Id, m.User.FirstName, m.User.LastName, m.User.DisplayName, m.User.SSN, m.User.Email, m.User.Created, m.User.Deleted),
                m.From, m.Thru));

            return new ItemsResult<ProjectMembershipDto>(dtos, totalItems);
        }
    }
}