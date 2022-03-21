﻿
using MediatR;

using YourCompany.TimeReport.Application.Common.Interfaces;
using YourCompany.TimeReport.Domain.Entities;

namespace YourCompany.TimeReport.Application.Users.Commands;

public class CreateUserCommand : IRequest<UserDto>
{
    public CreateUserCommand(string? id, string firstName, string lastName, string? displayName, string ssn, string email)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        DisplayName = displayName;
        Ssn = ssn;
        Email = email;
    }

    public string? Id { get; }

    public string FirstName { get; }

    public string LastName { get; }

    public string? DisplayName { get; }

    public string Ssn { get; }

    public string Email { get; }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly ITimeReportContext _context;

        public CreateUserCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Id = request.Id ?? Guid.NewGuid().ToString(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                DisplayName = request.DisplayName,
                SSN = request.Ssn,
                Email = request.Email
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync(cancellationToken);

            return new UserDto(user.Id, user.FirstName, user.LastName, user.DisplayName, user.SSN, user.Email, user.Created, user.Deleted);
        }
    }
}