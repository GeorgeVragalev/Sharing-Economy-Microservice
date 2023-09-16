using DAL.Entity.Enums;

namespace UserAPI.Models;

public class UserModel
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Email { get; set; }
    
    public string Phone { get; set; }
    
    public Gender Gender { get; set; }
    
    public DateTime Birthday { get; set; }
}