﻿
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourCompany.TimeReport.Application.Common.Interfaces;
using YourCompany.TimeReport.Application.Projects;

namespace YourCompany.TimeReport.Application.Activities.Queries;

public class GetActivityQuery : IRequest<ActivityDto>
{
    public GetActivityQuery(string activityid)
    {
        ActivityId = activityid;
    }

    public string ActivityId { get; }

    public class GetActivityQueryHandler : IRequestHandler<GetActivityQuery, ActivityDto>
    {
        private readonly ITimeReportContext _context;

        public GetActivityQueryHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ActivityDto> Handle(GetActivityQuery request, CancellationToken cancellationToken)
        {
            var activity = await _context.Activities
               .Include(x => x.Project)
               .AsNoTracking()
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == request.ActivityId, cancellationToken);

            if (activity is null)
            {
                throw new Exception();
            }

            return new ActivityDto(activity.Id, activity.Name, activity.Description, activity.HourlyRate, new ProjectDto(activity.Project.Id, activity.Project.Name, activity.Project.Description));
        }
    }
}