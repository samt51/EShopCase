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
    
    public Users(int Id, string email, string password, int roleId,string firstName, string lastName)
    {
        this.Id = Id;
        this.Email = email;
        this.Password = password;
        this.RoleId = roleId;
        this.FirstName = firstName;
        this.LastName = lastName;
    }
}
