using Drippy_Dropper.API.Models.Context;
using Drippy_Dropper.API.Models.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Drippy_Dropper.API.Functions.Queries.FolderQueries
{
    public class GetFoldersByUserQuery : IRequest<List<FolderDTO>>
    {
        public Guid OwnerId { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
    }

    public class GetFoldersByUserQueryHandler : IRequestHandler<GetFoldersByUserQuery, List<FolderDTO>>
    {
        private readonly DrippyDropperDb _context;

        public GetFoldersByUserQueryHandler(DrippyDropperDb context)
        {
            _context = context;
        }

        public async Task<List<FolderDTO>> Handle(GetFoldersByUserQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Folders
                .Where(f => f.OwnerId == request.OwnerId)
                .AsQueryable();

            if (request.CreatedAfter.HasValue)
                query = query.Where(f => f.CreatedAt >= request.CreatedAfter.Value);

            if (request.CreatedBefore.HasValue)
                query = query.Where(f => f.CreatedAt <= request.CreatedBefore.Value);

            var folders = await query.ToListAsync(cancellationToken);

            return folders.Select(f => new FolderDTO
            {
                FolderId = f.FolderId,
                Name = f.Name,
                Path = f.Path,
                ParentFolderId = f.ParentFolderId,
                FileCount = f.Files?.Count ?? 0,
                OwnerId = f.OwnerId,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt
            }).ToList();
        }
    }


}
