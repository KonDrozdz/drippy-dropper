using Drippy_Dropper.API.Configurations;
using Drippy_Dropper.API.Configurations.Drippy_Dropper.API.Configurations;
using Drippy_Dropper.API.Models.Context;
using MediatR;

namespace Drippy_Dropper.API.Functions.Commands.FileCommands
{
    public class AddedFileCommandDTO
    {
        public Guid FileId { get; set; }
        public string Name { get; set; }
        public Guid FolderId { get; set; }
        public Guid OwnerId { get; set; }
        public long Size { get; set; }
        public string ContentType { get; set; }
        public string Path { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AddFileCommand : IRequest<AddedFileCommandDTO>
    {
        public IFormFile File { get; set; }
        public Guid FolderId { get; set; }
        public Guid OwnerId { get; set; }
    }

    public class AddFileCommandHandler : IRequestHandler<AddFileCommand, AddedFileCommandDTO>
    {
        private readonly DrippyDropperDb _context;

        public AddFileCommandHandler(DrippyDropperDb context)
        {
            _context = context;
        }

        public async Task<AddedFileCommandDTO> Handle(AddFileCommand request, CancellationToken cancellationToken)
        {
            var folder = await _context.Folders.FindAsync(request.FolderId, cancellationToken);
            if (folder == null || folder.OwnerId != request.OwnerId)
                throw new Exception("Nie znaleziono folderu.");

            var folderPath = folder.Path;
            if (!Directory.Exists(folderPath))
                throw new Exception("Folder nie istnieje.");

            var filePath = Path.Combine(folderPath, request.File.FileName);

            if (File.Exists(filePath))
                throw new Exception("Plik o podanej nazwie już istnieje.");

            await using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
            {
                await request.File.CopyToAsync(stream, cancellationToken);
            }

            var file = new Models.Entities.File()
            {
                Name = request.File.FileName,
                FolderId = request.FolderId,
                OwnerId = request.OwnerId,
                Size = request.File.Length,
                ContentType = request.File.ContentType,
                Path = filePath,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Files.Add(file);
            await _context.SaveChangesAsync(cancellationToken);

            return new AddedFileCommandDTO
            {
                FileId = file.FileId,
                Name = file.Name,
                FolderId = file.FolderId,
                OwnerId = file.OwnerId,
                Size = file.Size,
                ContentType = file.ContentType,
                Path = file.Path,
                CreatedAt = file.CreatedAt
            };
        }
    }
}
