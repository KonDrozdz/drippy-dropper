using Drippy_Dropper.API.Models.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Drippy_Dropper.API.Functions.Queries.FileQueries
{
    public class FileDetailsDTO
    {
        public Guid FileId { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public string ContentType { get; set; }
        public string Path { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class GetFileQuery : IRequest<FileDetailsDTO>
    {
        public Guid FileId { get; set; }
        public Guid OwnerId { get; set; }
    }

    public class GetFileDetailsQueryHandler : IRequestHandler<GetFileQuery, FileDetailsDTO>
    {
        private readonly DrippyDropperDb _context;

        public GetFileDetailsQueryHandler(DrippyDropperDb context)
        {
            _context = context;
        }

        public async Task<FileDetailsDTO> Handle(GetFileQuery request, CancellationToken cancellationToken)
        {
            var file = await _context.Files
                .Where(f => f.FileId == request.FileId && f.OwnerId == request.OwnerId)
                .Select(f => new FileDetailsDTO
                {
                    FileId = f.FileId,
                    Name = f.Name,
                    Size = f.Size,
                    ContentType = f.ContentType,
                    Path = f.Path,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (file == null)
                throw new Exception("Nie znaleziono pliku.");

            return file;
        }
    }
}
