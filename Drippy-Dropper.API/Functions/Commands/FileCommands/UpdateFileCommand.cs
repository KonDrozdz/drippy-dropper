using Drippy_Dropper.API.Models.Context;
using MediatR;

namespace Drippy_Dropper.API.Functions.Commands.FileCommands
{
    public class UpdatedFileCommandDTO
    {
        public Guid FileId { get; set; }
        public string Name { get; set; }
        public Guid FolderId { get; set; }
        public Guid OwnerId { get; set; }
        public long Size { get; set; }
        public string ContentType { get; set; }
        public string Path { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class UpdateFileCommand : IRequest<UpdatedFileCommandDTO>
    {
        public Guid FileId { get; set; }
        public string? NewName { get; set; }
        public Guid? NewFolderId { get; set; }
        public Guid OwnerId { get; set; }
    }

    public class UpdateFileCommandHandler : IRequestHandler<UpdateFileCommand, UpdatedFileCommandDTO>
    {
        private readonly DrippyDropperDb _context;

        public UpdateFileCommandHandler(DrippyDropperDb context)
        {
            _context = context;
        }

        public async Task<UpdatedFileCommandDTO> Handle(UpdateFileCommand request, CancellationToken cancellationToken)
        {
            var file = await _context.Files.FindAsync(request.FileId, cancellationToken);
            if (file == null || file.OwnerId != request.OwnerId)
                throw new Exception("Nie znaleziono pliku lub brak dostępu.");

            string newPath = file.Path;


            if (!string.IsNullOrEmpty(request.NewName))
            {
                var directory = Path.GetDirectoryName(file.Path) ?? string.Empty;
                newPath = Path.Combine(directory, request.NewName);

                if (File.Exists(file.Path))
                    File.Move(file.Path, newPath);

                file.Name = request.NewName;
            }


            if (request.NewFolderId.HasValue)
            {
                var newFolder = await _context.Folders.FindAsync(request.NewFolderId.Value, cancellationToken);
                if (newFolder == null || newFolder.OwnerId != request.OwnerId)
                    throw new Exception("Nie znaleziono folderu lub brak dostępu.");

                var newDirectory = newFolder.Path;
                newPath = Path.Combine(newDirectory, Path.GetFileName(file.Path));

                if (File.Exists(file.Path))
                    File.Move(file.Path, newPath);

                file.FolderId = request.NewFolderId.Value;
            }

            file.Path = newPath;
            file.UpdatedAt = DateTime.UtcNow;

            _context.Files.Update(file);
            await _context.SaveChangesAsync(cancellationToken);

            return new UpdatedFileCommandDTO
            {
                FileId = file.FileId,
                Name = file.Name,
                FolderId = file.FolderId,
                OwnerId = file.OwnerId,
                Size = file.Size,
                ContentType = file.ContentType,
                Path = file.Path,
                UpdatedAt = file.UpdatedAt
            };
        }
    }

}
