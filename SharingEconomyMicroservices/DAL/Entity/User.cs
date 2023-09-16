using DAL.Entity.Enums;

namespace DAL.Entity;

public class User : BaseEntity
{
    public string Name { get; set; }
    
    public string Email { get; set; }
    public string Password { get; set; }
    
    public string Phone { get; set; }
    
    public virtual Gender Gender { get; set; }
    
    public DateTime Birthday { get; set; }
}