using EShopCase.Domain.Common;

namespace EShopCase.Domain.Entities;

public class Users : EntityBase
{
    public Users()
    {

    }
    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public int RoleId { get; set; }
    public Roles Role { get; set; }
    
    public Users(int Id, string email, string password, int roleId)
    {
        this.Id = Id;
        this.Email = email;
        this.Password = password;
        this.RoleId = roleId;
    }
}
