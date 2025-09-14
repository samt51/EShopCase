using EShopCase.Application.Bases;
using MediatR;

namespace EShopCase.Application.Features.ProductsFeature.Queries.GetAllProduct;

public class GetAllProductQueryRequest : IRequest<ResponseDto<PagedResult<GetAllProductQueryResponse>>>
{
  
    public string? Name { get; init; }   
    public decimal? PriceMin { get; init; }
    public decimal? PriceMax { get; init; }
    public int? CategoryId { get; init; }
 
    public string? SortBy { get; init; }   
    public string? SortDir { get; init; } 
    
    public int Page { get; init; } = 1;       
    public int PageSize { get; init; } = 20;   
}