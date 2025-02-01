using Drippy_Dropper.API.Configurations.Drippy_Dropper.API.Configurations;
using Drippy_Dropper.API.Models.Context;
using Drippy_Dropper.API.Models.Entities;
using MediatR;

namespace Drippy_Dropper.API.Functions.Commands.UserCommands
{
    public class RegisteredUserCommandDTO
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class RegisterCommand : IRequest<RegisteredUserCommandDTO>
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisteredUserCommandDTO>
    {
        private readonly DrippyDropperDb _context;
        private readonly StorageSettings _storageSettings;

        public RegisterCommandHandler(DrippyDropperDb context, StorageSettings storageSettings)
        {
            _context = context;
            _storageSettings = storageSettings;
        }

        public async Task<RegisteredUserCommandDTO> Handle(RegisterCommand request,
            CancellationToken cancellationToken)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                if (_context.Users.Any(u => u.Email == request.Email))
                    throw new Exception("Użytkownik z podanym email już istnieje.");

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);


                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = hashedPassword,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync(cancellationToken);

                var userBasePath = _storageSettings.GetUserBasePath(user.UserId);

                var userFolder = new Folder
                {
                    Name = user.UserId.ToString(),
                    ParentFolderId = null,
                    OwnerId = user.UserId,
                    Path = userBasePath,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Folders.Add(userFolder);
                await _context.SaveChangesAsync(cancellationToken);


                await transaction.CommitAsync(cancellationToken);


                return new RegisteredUserCommandDTO
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt
                };
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }

}