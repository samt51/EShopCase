using EShopCase.Application.Dtos.CategoryDtos;

namespace EShopCase.Application.Features.ProductsFeature.Queries.GetByIdProduct;

public class GetByIdProductQueryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; }  = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public CategoryResponseDto CategoryResponse { get; set; } 
}