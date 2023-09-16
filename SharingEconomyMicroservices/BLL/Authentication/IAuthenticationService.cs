namespace BLL.Authentication;

public interface IAuthenticationService
{
    public Task<string> GenerateToken(DAL.Entity.User user);
    public Task<bool> ValidateToken(string token);
    public Task<bool> VerifyPassword(string password);
}