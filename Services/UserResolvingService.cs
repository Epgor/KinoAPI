using KinoAPI.Models;
using System.Security.Claims;

namespace KinoAPI.Services;

public static class UserResolvingService
{
    public static User Resolve(HttpContext context)
    {
        var claims = context.User.Identities.First().Claims;
        var userName = claims.Where(c => c.Type == ClaimTypes.Name)
            .Select(c => c.Value).FirstOrDefault();
        var userRole = claims.Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value).FirstOrDefault();
        return new User()
        {
            Name = userName,
            Role = userRole
        };

    }
}
