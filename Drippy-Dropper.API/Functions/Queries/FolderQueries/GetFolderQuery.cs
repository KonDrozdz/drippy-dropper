using Drippy_Dropper.API.Models.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Drippy_Dropper.API.Functions.Queries.FolderQueries
{
    public class FolderDetailsDTO
    {
        public Guid FolderId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public Guid? ParentFolderId { get; set; }
        public string? ParentFolderName { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<SubFolderDTO> SubFolders { get; set; } = new();
        public List<FileDTO> Files { get; set; } = new();
    }

    public class SubFolderDTO
    {
        public Guid FolderId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class GetFolderQuery : IRequest<FolderDetailsDTO>
    {
        public Guid FolderId { get; set; }
        public Guid OwnerId { get; set; }
    }

    public class GetFolderQueryHandler : IRequestHandler<GetFolderQuery, FolderDetailsDTO>
    {
        private readonly DrippyDropperDb _context;

        public GetFolderQueryHandler(DrippyDropperDb context)
        {
            _context = context;
        }

        public async Task<FolderDetailsDTO> Handle(GetFolderQuery request, CancellationToken cancellationToken)
        {
            var folder = await _context.Folders
                .Include(f => f.ParentFolder)
                .Include(f => f.SubFolders)
                .Include(f => f.Files)
                .FirstOrDefaultAsync(f => f.FolderId == request.FolderId && f.OwnerId == request.OwnerId, cancellationToken);

            if (folder == null)
            {
                throw new Exception("Folder nie został znaleziony.");
            }

            return new FolderDetailsDTO
            {
                FolderId = folder.FolderId,
                Name = folder.Name,
                Path = folder.Path,
                ParentFolderId = folder.ParentFolderId,
                ParentFolderName = folder.ParentFolder?.Name,
                CreatedAt = folder.CreatedAt,
                SubFolders = folder.SubFolders!.Select(sf => new SubFolderDTO
                {
                    FolderId = sf.FolderId,
                    Name = sf.Name,
                    CreatedAt = sf.CreatedAt
                }).ToList(),
                Files = folder.Files!.Select(file => new FileDTO
                {
                    FileId = file.FileId,
                    Name = file.Name,
                    ContentType = file.ContentType,
                    Size = file.Size,
                    CreatedAt = file.CreatedAt
                }).ToList()
            };
        }
    }


}
