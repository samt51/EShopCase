using EShopCase.Application.Dtos.CategoryDtos;

namespace EShopCase.Application.Features.ProductsFeature.Queries.GetAllProduct;

public class GetAllProductQueryResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; } 
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public CategoryResponseDto CategoryResponse { get; set; }
}