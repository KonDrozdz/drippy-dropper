using Drippy_Dropper.API.Models.Entities;

namespace Drippy_Dropper.API.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }

}
