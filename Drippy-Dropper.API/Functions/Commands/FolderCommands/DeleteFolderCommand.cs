using Drippy_Dropper.API.Models.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Drippy_Dropper.API.Functions.Commands.FolderCommands
{
    public class DeletedFolderCommandDTO
    {
        public Guid FolderId { get; set; }
        public Guid OwnerId { get; set; }
    }

    public class DeleteFolderCommand : IRequest<DeletedFolderCommandDTO>
    {
        public Guid FolderId { get; set; }
        public Guid OwnerId { get; set; }
    }

    public class DeleteFolderCommandHandler : IRequestHandler<DeleteFolderCommand, DeletedFolderCommandDTO>
    {
        private readonly DrippyDropperDb _context;

        public DeleteFolderCommandHandler(DrippyDropperDb context)
        {
            _context = context;
        }

        public async Task<DeletedFolderCommandDTO> Handle(DeleteFolderCommand request, CancellationToken cancellationToken)
        {
            var folder = await _context.Folders
                .Include(f => f.SubFolders)
                .Include(f => f.Files)
                .FirstOrDefaultAsync(f => f.FolderId == request.FolderId, cancellationToken);

            if (folder == null || folder.OwnerId != request.OwnerId)
                throw new Exception("Folder nie został znaleziony lub brak uprawnień.");

            foreach (var file in folder.Files)
            {
                if (File.Exists(file.Path))
                    File.Delete(file.Path);
            }


            if (Directory.Exists(folder.Path))
                Directory.Delete(folder.Path, recursive: true);

            _context.Folders.Remove(folder);
            await _context.SaveChangesAsync(cancellationToken);

            return new DeletedFolderCommandDTO
            {
                FolderId = folder.FolderId,
                OwnerId = folder.OwnerId
            };
        }
    }

}
