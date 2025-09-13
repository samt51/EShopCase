using EShopCase.Domain.Common;

namespace EShopCase.Domain.Entities;

public class Category : EntityBase
{
    public Category()
    {
        
    }
    public Category(int id,string name)
    {
        this.Id = id;
        this.Name = name;   
    }
    public Category(string name)
    {
        this.Name = name;   
    }
    public string Name { get; set; }
    public IList<Products> Products { get; set; }
}