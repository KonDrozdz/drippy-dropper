using Drippy_Dropper.API.Models.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Drippy_Dropper.API.Functions.Commands.FolderCommands
{
    public class UpdatedFolderCommandDTO
    {
        public Guid FolderId { get; set; }
        public string Name { get; set; }
        public Guid? ParentFolderId { get; set; }
        public Guid OwnerId { get; set; }
        public string FolderPath { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UpdateFolderCommand : IRequest<UpdatedFolderCommandDTO>
    {
        public Guid FolderId { get; set; }
        public string? NewName { get; set; }
        public Guid? NewParentFolderId { get; set; }
        public Guid OwnerId { get; set; }
    }

    public class UpdateFolderCommandHandler : IRequestHandler<UpdateFolderCommand, UpdatedFolderCommandDTO>
    {
        private readonly DrippyDropperDb _context;

        public UpdateFolderCommandHandler(DrippyDropperDb context)
        {
            _context = context;
        }

        public async Task<UpdatedFolderCommandDTO> Handle(UpdateFolderCommand request, CancellationToken cancellationToken)
        {
            var folder = await _context.Folders
                .Include(f => f.ParentFolder)
                .FirstOrDefaultAsync(f => f.FolderId == request.FolderId, cancellationToken);

            if (folder == null || folder.OwnerId != request.OwnerId)
                throw new Exception("Folder nie został znaleziony lub brak uprawnień.");

            string newPath = folder.Path;


            if (!string.IsNullOrEmpty(request.NewName))
            {
                var parentPath = Path.GetDirectoryName(folder.Path) ?? string.Empty;
                newPath = Path.Combine(parentPath, request.NewName);

                if (Directory.Exists(folder.Path))
                    Directory.Move(folder.Path, newPath);

                folder.Name = request.NewName;
            }

            if (request.NewParentFolderId.HasValue)
            {
                var newParentFolder = await _context.Folders.FindAsync(request.NewParentFolderId.Value);
                if (newParentFolder == null || newParentFolder.OwnerId != request.OwnerId)
                    throw new Exception("Nowy folder nadrzędny nie istnieje lub brak dostępu.");

                var newParentPath = newParentFolder.Path;
                newPath = Path.Combine(newParentPath, folder.Name);

                if (Directory.Exists(folder.Path))
                    Directory.Move(folder.Path, newPath);

                folder.ParentFolderId = request.NewParentFolderId;
            }


            folder.Path = newPath;
            folder.UpdatedAt = DateTime.UtcNow;

            _context.Folders.Update(folder);
            await _context.SaveChangesAsync(cancellationToken);

            return new UpdatedFolderCommandDTO
            {
                FolderId = folder.FolderId,
                Name = folder.Name,
                ParentFolderId = folder.ParentFolderId,
                OwnerId = folder.OwnerId,
                FolderPath = folder.Path,
                UpdatedAt = (DateTime)folder.UpdatedAt
            };
        }
    }

}
