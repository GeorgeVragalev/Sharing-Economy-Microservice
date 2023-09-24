namespace BLL.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly string SecretKey = "ThisIsASecretKeyThatIsAtLeast32Bytes000";

    public Task<string> GenerateToken(DAL.Entity.User user)
    {
        return Task.FromResult("false");
    }

    public Task<bool> ValidateToken(string token)
    {
        return Task.FromResult(false);
    }
    
    public Task<bool> VerifyPassword(string password)
    {
        // Verify a password against the stored hash.
        return Task.FromResult(false);
    }
}
