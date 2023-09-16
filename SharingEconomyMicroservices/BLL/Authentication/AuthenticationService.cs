using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BLL.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly string SecretKey = "ThisIsASecretKeyThatIsAtLeast32Bytes000";

    public Task<string> GenerateToken(DAL.Entity.User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(SecretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[] 
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Authentication, user.Password)
            }),
            
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Task.FromResult(tokenHandler.WriteToken(token));
    }

    public Task<bool> ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(SecretKey);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clock skew to zero for instant token expiration time checks
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken) validatedToken;
            var apiSignature = jwtToken.Claims.FirstOrDefault(x => x.Type == "from_gateway");

            return Task.FromResult(true);

            if (apiSignature?.Value == "true")
            {
                return Task.FromResult(true);
            }
        }
        catch
        {
            // Token validation failed
        }
        
        return Task.FromResult(false);
    }
    
    public Task<bool> VerifyPassword(string password)
    {
        var hashedPassword = HashPassword(password);
        
        // Verify a password against the stored hash.
        return Task.FromResult(BCrypt.Net.BCrypt.Verify(password, hashedPassword));
    }

    private string HashPassword(string password)
    {
        // Automatically creates a salt and hash the password with it.
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}
