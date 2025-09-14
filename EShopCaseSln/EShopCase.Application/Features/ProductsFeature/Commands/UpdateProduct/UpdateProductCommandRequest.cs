using EShopCase.Application.Bases;
using MediatR;

namespace EShopCase.Application.Features.ProductsFeature.Commands.UpdateProduct;

public class UpdateProductCommandRequest :IRequest<ResponseDto<UpdateProductCommandResponse>>
{
    public UpdateProductCommandRequest(int id,string name, string description, decimal price, int stock, int categoryId)
    {
        this.Id = id;   
        this.Name = name;
        this.Description = description;
        this.Price = price;
        this.Stock = stock;
        this.CategoryId = categoryId;
    }

    public int Id { get; set; }
    public string Name { get; }
    public string Description { get;  } 
    public decimal Price { get; }
    public int Stock { get;  }
    public int CategoryId { get;  }
}