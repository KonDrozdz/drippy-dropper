using Drippy_Dropper.API.Models.Context;
using Drippy_Dropper.API.Models.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Drippy_Dropper.API.Functions.Queries.FileQueries
{
    public class GetFilesByUserQuery : IRequest<List<FileDTO>>
    {
        public Guid OwnerId { get; set; }
        public string? ContentType { get; set; }
        public long? MinSize { get; set; }
        public long? MaxSize { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
    }

    public class GetFilesByUserQueryHandler : IRequestHandler<GetFilesByUserQuery, List<FileDTO>>
    {
        private readonly DrippyDropperDb _context;

        public GetFilesByUserQueryHandler(DrippyDropperDb context)
        {
            _context = context;
        }

        public async Task<List<FileDTO>> Handle(GetFilesByUserQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Files
                .Where(f => f.OwnerId == request.OwnerId)
                .AsQueryable();


            if (!string.IsNullOrEmpty(request.ContentType))
                query = query.Where(f => f.ContentType == request.ContentType);

            if (request.MinSize.HasValue)
                query = query.Where(f => f.Size >= request.MinSize.Value);

            if (request.MaxSize.HasValue)
                query = query.Where(f => f.Size <= request.MaxSize.Value);

            if (request.CreatedAfter.HasValue)
                query = query.Where(f => f.CreatedAt >= request.CreatedAfter.Value);

            if (request.CreatedBefore.HasValue)
                query = query.Where(f => f.CreatedAt <= request.CreatedBefore.Value);


            return await query
                .Select(f => new FileDTO
                {
                    FileId = f.FileId,
                    Name = f.Name,
                    ContentType = f.ContentType,
                    Size = f.Size,
                    Path = f.Path,
                    CreatedAt = f.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }
    }

}