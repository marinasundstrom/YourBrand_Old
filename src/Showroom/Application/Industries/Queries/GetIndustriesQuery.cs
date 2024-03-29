﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Application.Common.Models;
using YourBrand.Showroom.Application.Industries;
using YourBrand.Showroom.Domain.Entities;
using YourBrand.Showroom.Domain.Exceptions;

namespace YourBrand.Showroom.Application.Industries.Queries;

public record GetIndustriesQuery(int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<Results<IndustryDto>>
{
    class GetIndustriesQueryHandler : IRequestHandler<GetIndustriesQuery, Results<IndustryDto>>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;

        public GetIndustriesQueryHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<Results<IndustryDto>> Handle(GetIndustriesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Industry> result = _context
                    .Industries
                     //.OrderBy(o => o.Created)
                     .AsNoTracking()
                     .AsQueryable();

            if (request.SearchString is not null)
            {
                result = result.Where(ca => ca.Name.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalCount = await result.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                result = result.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? Showroom.Application.SortDirection.Descending : Showroom.Application.SortDirection.Ascending);
            }
            else 
            {
                result = result.OrderBy(x => x.Name);
            }

            var items = await result
                .Skip((request.Page) * request.PageSize)
                .Take(request.PageSize)
                .ToArrayAsync(cancellationToken);

            return new Results<IndustryDto>(items.Select(cp => cp.ToDto()), totalCount);
        }
    }
}
