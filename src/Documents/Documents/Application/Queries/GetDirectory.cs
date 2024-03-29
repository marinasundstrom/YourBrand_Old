﻿using YourBrand.Documents.Infrastructure.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Documents.Application.Queries;

public record GetDirectory(string Path) : IRequest<DirectoryDto?>
{
    public class Handler : IRequestHandler<GetDirectory, DirectoryDto?>
    {
        private readonly DocumentsContext _context;
        private readonly IUrlResolver _urlResolver;

        public Handler(DocumentsContext context, IUrlResolver urlResolver)
        {
            _context = context;
            _urlResolver = urlResolver;
        }

        public async Task<DirectoryDto?> Handle(GetDirectory request, CancellationToken cancellationToken)
        {
            var path = request.Path?.Trim() ?? string.Empty;

            var directory = await _context.Directories
                .Include(x => x.Parent)
                .Include(x => x.Directories)
                .Include(x => x.Documents)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Name == path, cancellationToken);

            return directory is null
                ? null
                : directory.ToDto(_urlResolver.GetUrl); 
        }
    }
}
