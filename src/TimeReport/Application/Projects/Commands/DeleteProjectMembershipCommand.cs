﻿
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Exceptions;

namespace YourBrand.TimeReport.Application.Projects.Commands;

public record DeleteProjectMembershipCommand(string ProjectId, string MembershipId) : IRequest
{
    public class DeleteProjectMembershipCommandHandler : IRequestHandler<DeleteProjectMembershipCommand>
    {
        private readonly ITimeReportContext _context;

        public DeleteProjectMembershipCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteProjectMembershipCommand request, CancellationToken cancellationToken)
        {
            var project = await _context.Projects
                .Include(p => p.Organization)
                .Include(p => p.Memberships)
                .ThenInclude(m => m.User)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
            {
                throw new ProjectNotFoundException(request.ProjectId);
            }

            var m = project.Memberships.FirstOrDefault(x => x.Id == request.MembershipId);

            if (m is null)
            {
                throw new ProjectMembershipNotFoundException(request.MembershipId);
            }

            _context.ProjectMemberships.Remove(m);

            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}