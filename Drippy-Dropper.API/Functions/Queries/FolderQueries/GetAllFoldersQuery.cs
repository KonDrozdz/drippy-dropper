using Drippy_Dropper.API.Models.Context;
using Drippy_Dropper.API.Models.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Drippy_Dropper.API.Functions.Queries.FolderQueries
{
    public class GetAllFoldersQuery : IRequest<List<FolderDTO>>
    {
        public Guid OwnerId { get; set; }
    }

    public class GetAllFoldersQueryHandler : IRequestHandler<GetAllFoldersQuery, List<FolderDTO>>
    {
        private readonly DrippyDropperDb _context;

        public GetAllFoldersQueryHandler(DrippyDropperDb context)
        {
            _context = context;
        }

        public async Task<List<FolderDTO>> Handle(GetAllFoldersQuery request, CancellationToken cancellationToken)
        {
            return await _context.Folders
                .Where(f => f.OwnerId == request.OwnerId)
                .Select(f => new FolderDTO
                {
                    FolderId = f.FolderId,
                    Name = f.Name,
                    Path = f.Path,
                    ParentFolderId = f.ParentFolderId,
                    FileCount = f.Files.Count,
                    OwnerId = f.OwnerId,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt,
                })
                .ToListAsync(cancellationToken);
        }
    }

}
