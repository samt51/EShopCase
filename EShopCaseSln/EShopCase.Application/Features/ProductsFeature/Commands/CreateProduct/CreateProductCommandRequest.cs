using EShopCase.Application.Bases;
using MediatR;

namespace EShopCase.Application.Features.ProductsFeature.Commands.CreateProduct;

public class CreateProductCommandRequest : IRequest<ResponseDto<CreateProductCommandResponse>>
{
    public CreateProductCommandRequest(string name, string description, decimal price, int stock, int categoryId)
    {
        this.Name = name;
        this.Description = description;
        this.Price = price;
        this.Stock = stock;
        this.CategoryId = categoryId;
    }
    public string Name { get; }
    public string Description { get;  } 
    public decimal Price { get; }
    public int Stock { get;  }
    public int CategoryId { get;  }
}