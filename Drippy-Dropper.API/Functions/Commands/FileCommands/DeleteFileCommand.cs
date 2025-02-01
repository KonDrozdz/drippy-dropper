using Drippy_Dropper.API.Models.Context;
using MediatR;

namespace Drippy_Dropper.API.Functions.Commands.FileCommands
{
    public class DeletedFileCommandDTO : IRequest
    {
        public Guid FileId { get; set; }
        public Guid OwnerId { get; set; }
    }

    public class DeleteFileCommand : IRequest<DeletedFileCommandDTO>
    {
        public Guid FileId { get; set; }
        public Guid OwnerId { get; set; }
    }

    public class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand, DeletedFileCommandDTO>
    {
        private readonly DrippyDropperDb _context;

        public DeleteFileCommandHandler(DrippyDropperDb context)
        {
            _context = context;
        }

        public async Task<DeletedFileCommandDTO> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
        {
            var file = await _context.Files.FindAsync(request.FileId, cancellationToken);
            if (file == null || file.OwnerId != request.OwnerId)
                throw new Exception("Nie znaleziono pliku lub brak dostępu.");

            if (File.Exists(file.Path))
                File.Delete(file.Path);

            _context.Files.Remove(file);
            await _context.SaveChangesAsync(cancellationToken);

            return new DeletedFileCommandDTO
            {
                FileId = file.FileId,
                OwnerId = file.OwnerId
            };
        }
    }

}
