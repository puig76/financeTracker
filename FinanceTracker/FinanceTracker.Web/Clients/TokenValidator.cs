using System;
using System.IdentityModel.Tokens.Jwt;

namespace FinanceTracker.Web.Clients;

public static class TokenValidator
{
    public static bool IsTokenExpired(string token)
    {
        JwtSecurityToken jwtToken = GetJWT(token);
        var expiration = jwtToken.ValidTo;

        return expiration < DateTime.UtcNow;
    }

    public static JwtSecurityToken GetJWT(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        
        return jwtToken;
    }
}
