using Drippy_Dropper.API.Models.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Drippy_Dropper.API.Functions.Queries.FileQueries
{
    public class FileDownloadDTO
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
    }

    public class GetFileDownloadQuery : IRequest<FileDownloadDTO>
    {
        public Guid FileId { get; set; }
        public Guid OwnerId { get; set; }
    }

    public class GetFileDownloadQueryHandler : IRequestHandler<GetFileDownloadQuery, FileDownloadDTO>
    {
        private readonly DrippyDropperDb _context;

        public GetFileDownloadQueryHandler(DrippyDropperDb context)
        {
            _context = context;
        }

        public async Task<FileDownloadDTO> Handle(GetFileDownloadQuery request, CancellationToken cancellationToken)
        {
            var file = await _context.Files
                .Where(f => f.FileId == request.FileId && f.OwnerId == request.OwnerId)
                .FirstOrDefaultAsync(cancellationToken);

            if (file == null)
                throw new Exception("Nie znaleziono pliku.");

            if (!System.IO.File.Exists(file.Path))
                throw new FileNotFoundException("Plik nie istnieje na serwerze.");

            var fileContent = await System.IO.File.ReadAllBytesAsync(file.Path, cancellationToken);

            return new FileDownloadDTO
            {
                Name = file.Name,
                ContentType = file.ContentType,
                Content = fileContent
            };
        }
    }

}
