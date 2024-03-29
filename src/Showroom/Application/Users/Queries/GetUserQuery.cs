﻿
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Domain.Exceptions;

namespace YourBrand.Showroom.Application.Users.Queries;

public record GetUserQuery(string UserId) : IRequest<UserDto>
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto>
    { 
        readonly IShowroomContext _context;

        public GetUserQueryHandler(IShowroomContext context)
        {
            _context = context;
        }

        public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                return null!;
            }

            return new UserDto(user.Id, user.FirstName, user.LastName, user.DisplayName, user.SSN, user.Email, user.Created, user.LastModified);
        }
    }
}