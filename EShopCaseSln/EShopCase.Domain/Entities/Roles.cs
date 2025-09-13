using EShopCase.Domain.Common;

namespace EShopCase.Domain.Entities;

public class Roles :EntityBase  
{
    public Roles()
    {

    }
    public Roles(string name, int id)
    {
        this.Name = name;
        this.Id = id;
    }
    public string Name { get; set; }
}