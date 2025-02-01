using Drippy_Dropper.API.Models.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Drippy_Dropper.API.Functions.Queries.UserQueries
{
    public class UserDTO
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int FolderCount { get; set; }
        public int FileCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class GetUserQuery : IRequest<UserDTO>
    {
        public Guid UserId { get; set; }
    }

    public class GetUserDetailsQueryHandler : IRequestHandler<GetUserQuery, UserDTO>
    {
        private readonly DrippyDropperDb _context;

        public GetUserDetailsQueryHandler(DrippyDropperDb context)
        {
            _context = context;
        }

        public async Task<UserDTO> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Where(u => u.UserId == request.UserId)
                .Select(u => new UserDTO
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    FolderCount = u.Folders!.Count,
                    FileCount = u.Files!.Count,
                    CreatedAt = u.CreatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                throw new Exception("Nie znaleziono użytkownika.");

            return user;
        }
    }
}
