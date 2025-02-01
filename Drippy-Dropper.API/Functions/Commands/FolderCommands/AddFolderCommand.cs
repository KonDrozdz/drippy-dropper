using Drippy_Dropper.API.Configurations;
using Drippy_Dropper.API.Configurations.Drippy_Dropper.API.Configurations;
using Drippy_Dropper.API.Models.Context;
using Drippy_Dropper.API.Models.Entities;
using MediatR;

namespace Drippy_Dropper.API.Functions.Commands.FolderCommands
{
    public class AddedFolderCommand
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ParentFolderId { get; set; }
        public Guid OwnerId { get; set; }
        public string FolderPath { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AddFolderCommand : IRequest<AddedFolderCommand>
    {
        public string Name { get; set; }
        public Guid? ParentFolderId { get; set; }
        public Guid OwnerId { get; set; }
    }

    public class AddFolderCommandHandler : IRequestHandler<AddFolderCommand, AddedFolderCommand>
    {
        private readonly DrippyDropperDb _context;
        private readonly StorageSettings _storageSettings;

        public AddFolderCommandHandler(DrippyDropperDb context, StorageSettings storageSettings)
        {
            _context = context;
            _storageSettings = storageSettings;
        }

        public async Task<AddedFolderCommand> Handle(AddFolderCommand request, CancellationToken cancellationToken)
        {
            string parentFolderPath;
            var parentFolderId = request.ParentFolderId;

            if (request.ParentFolderId.HasValue)
            {
                var parentFolder = await _context.Folders.FindAsync(request.ParentFolderId.Value, cancellationToken);
                if (parentFolder == null)
                    throw new Exception("Nie znaleziono nadrzednego folderu.");

                parentFolderPath = parentFolder.Path;
                parentFolderId = parentFolder.FolderId;
            }
            else
            {
                parentFolderPath = _storageSettings.GetUserBasePath(request.OwnerId);
            }

            var folderPath = Path.Combine(parentFolderPath, request.Name);
            if (Directory.Exists(folderPath))
                throw new Exception("Folder o podanej nazwie już istnieje.");

            Directory.CreateDirectory(folderPath);

            var folder = new Folder
            {
                Name = request.Name,
                ParentFolderId = parentFolderId,
                OwnerId = request.OwnerId,
                Path = folderPath,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Folders.Add(folder);
            await _context.SaveChangesAsync(cancellationToken);

            return new AddedFolderCommand
            {
                Id = folder.FolderId,
                Name = folder.Name,
                ParentFolderId = folder.ParentFolderId,
                OwnerId = folder.OwnerId,
                FolderPath = folder.Path,
                CreatedAt = folder.CreatedAt
            };
        }
    }

}