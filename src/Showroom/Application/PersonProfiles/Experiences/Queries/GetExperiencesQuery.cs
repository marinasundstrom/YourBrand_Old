﻿
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Application.Common.Models;
using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Application.PersonProfiles.Experiences.Queries;

public record GetExperiencesQuery(int Page = 0, int? PageSize = 10, string? PersonProfileId = null, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<Results<ExperienceDto>>
{
    class GetExperiencesQueryHandler : IRequestHandler<GetExperiencesQuery, Results<ExperienceDto>>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;

        public GetExperiencesQueryHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<Results<ExperienceDto>> Handle(GetExperiencesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<PersonProfileExperience> result = _context
                    .PersonProfileExperiences
                    .OrderByDescending(x => x.StartDate)
                    .ThenByDescending(x => x.EndDate)
                    .AsNoTracking()
                    .AsQueryable();

            if (!string.IsNullOrEmpty(request.PersonProfileId))
            {
                result = result.Where(e => e.PersonProfile.Id == request.PersonProfileId);
            }

            if (request.SearchString is not null)
            {
                result = result.Where(p =>
                    p.Company.Name.ToLower().Contains(request.SearchString.ToLower())
                    || p.Location!.ToLower().Contains(request.SearchString.ToLower())
                    || p.Title.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalCount = await result.CountAsync(cancellationToken);

            result = result
                .Include(x => x.Employment)
                .ThenInclude(x => x.Employer)
                .Include(x => x.Company)
                .ThenInclude(x => x.Industry)
                .Include(x => x.Skills)
                .ThenInclude(x => x.PersonProfileSkill)
                .ThenInclude(x => x.Skill)
                .ThenInclude(x => x.Area)
                .ThenInclude(x => x.Industry);

            if (request.SortBy is not null)
            {
                result = result.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? Showroom.Application.SortDirection.Descending : Showroom.Application.SortDirection.Ascending);
            }

            IQueryable<PersonProfileExperience> items = null!;

            if(request.PageSize is null) 
            {
                items = result.AsQueryable();
            }
            else 
            {
                items = result
                    .Skip((request.Page) * request.PageSize.GetValueOrDefault())
                    .Take(request.PageSize.GetValueOrDefault());
            }

            return new Results<ExperienceDto>(
                items.Select(e => e.ToDto()), 
                totalCount);
        }
    }
}
