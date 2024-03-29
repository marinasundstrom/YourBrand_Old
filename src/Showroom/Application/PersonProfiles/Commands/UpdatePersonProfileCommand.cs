﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Domain.Entities;
using YourBrand.Showroom.Domain.Exceptions;

namespace YourBrand.Showroom.Application.PersonProfiles.Commands;

public record UpdatePersonProfileCommand(string Id, UpdatePersonProfileDto PersonProfile) : IRequest<PersonProfileDto>
{
    class UpdatePersonProfileCommandHandler : IRequestHandler<UpdatePersonProfileCommand, PersonProfileDto>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;
        private readonly IUrlHelper _urlHelper;

        public UpdatePersonProfileCommandHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService,
            IUrlHelper urlHelper)
        {
            _context = context;
            this.currentUserService = currentUserService;
            _urlHelper = urlHelper;
        }

        public async Task<PersonProfileDto> Handle(UpdatePersonProfileCommand request, CancellationToken cancellationToken)
        {
            var personProfile = await _context.PersonProfiles.FindAsync(request.Id);
            if (personProfile is null)
            {
                return null;
            }

            personProfile.FirstName = request.PersonProfile.FirstName;
            personProfile.LastName = request.PersonProfile.LastName;
            personProfile.DisplayName = request.PersonProfile.DisplayName;
            personProfile.OrganizationId = request.PersonProfile.OrganizationId;
            personProfile.CompetenceAreaId = request.PersonProfile.CompetenceAreaId;
            personProfile.ManagerId = "";
            personProfile.ShortPresentation = "";
            personProfile.Presentation = "";

            if (personProfile.AvailableFromDate is not null)
            {
                personProfile.AvailableFromDate = personProfile.AvailableFromDate?.Date;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return personProfile.ToDto(_urlHelper);
        }
    }
}
