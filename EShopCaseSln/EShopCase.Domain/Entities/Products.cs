using EShopCase.Domain.Common;

namespace EShopCase.Domain.Entities;

public class Products : EntityBase
{
    public Products()
    {
        
    }
    public Products(string name,string description,decimal price,int stock,int categoryId)
    {
        this.Name = name;   
        this.Description = description; 
        this.Price = price; 
        this.Stock = stock;   
        this.CategoryId = categoryId;
    }
    public string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
}