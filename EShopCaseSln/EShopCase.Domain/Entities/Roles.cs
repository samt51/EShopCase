using EShopCase.Domain.Common;

namespace EShopCase.Domain.Entities;

public class Roles :EntityBase  
{
    public Roles()
    {

    }
    public Roles(int id,string name)
    {
        this.Id = id;
        this.Name = name;
    }
    public string Name { get; set; }
}