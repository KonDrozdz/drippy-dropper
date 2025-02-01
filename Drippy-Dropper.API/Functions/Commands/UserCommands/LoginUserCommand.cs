using Drippy_Dropper.API.Models.Context;
using Drippy_Dropper.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Drippy_Dropper.API.Functions.Commands.UserCommands
{
    public class LoginCommand : IRequest<string>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
    {
        private readonly DrippyDropperDb _context;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(DrippyDropperDb context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
            if (user == null || !user.IsActive)
                throw new Exception("Nie znaleziono takiego aktywnego konta.");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new Exception("Nieprawidłowy email lub hasło.");

            var token = _tokenService.GenerateToken(user);

            return token;
        }
    }

}
