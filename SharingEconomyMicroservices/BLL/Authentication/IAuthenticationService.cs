namespace BLL.Authentication;

public interface IAuthenticationService
{
    public string GenerateToken(DAL.Entity.User user);
    public bool ValidateToken(string token);
}