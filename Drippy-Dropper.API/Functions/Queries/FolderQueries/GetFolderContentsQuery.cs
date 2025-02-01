using Drippy_Dropper.API.Models.Context;
using Drippy_Dropper.API.Models.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Drippy_Dropper.API.Functions.Queries.FolderQueries
{
    public class FileDTO
    {
        public Guid FileId { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public string ContentType { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class FolderContentDTO
    {
        public Guid FolderId { get; set; }
        public string Name { get; set; }
        public List<FolderDTO> SubFolders { get; set; }
        public List<FileDTO> Files { get; set; }
    }

    public class GetFolderContentsQuery : IRequest<FolderContentDTO>
    {
        public Guid FolderId { get; set; }
        public Guid OwnerId { get; set; }
    }

    public class GetFolderContentsQueryHandler : IRequestHandler<GetFolderContentsQuery, FolderContentDTO>
    {
        private readonly DrippyDropperDb _context;

        public GetFolderContentsQueryHandler(DrippyDropperDb context)
        {
            _context = context;
        }

        public async Task<FolderContentDTO> Handle(GetFolderContentsQuery request, CancellationToken cancellationToken)
        {
            var folder = await _context.Folders
                .Include(f => f.SubFolders)
                .Include(f => f.Files)
                .FirstOrDefaultAsync(f => f.FolderId == request.FolderId && f.OwnerId == request.OwnerId, cancellationToken);

            if (folder == null)
            {
                throw new Exception("Folder nie został znaleziony.");
            }

            return new FolderContentDTO
            {
                FolderId = folder.FolderId,
                Name = folder.Name,
                SubFolders = folder.SubFolders!.Select(sf => new FolderDTO
                {
                    FolderId = sf.FolderId,
                    Name = sf.Name,
                    Path = sf.Path,
                    //  ParentFolderId = sf.ParentFolderId,
                    FileCount = sf.Files?.Count ?? 0,
                    OwnerId = sf.OwnerId,
                    CreatedAt = sf.CreatedAt,
                    UpdatedAt = sf.UpdatedAt
                }).ToList(),
                Files = folder.Files!.Select(file => new FileDTO
                {
                    FileId = file.FileId,
                    Name = file.Name,
                    Size = file.Size,
                    ContentType = file.ContentType,
                    CreatedAt = file.CreatedAt
                }).ToList()
            };
        }
    }


}
