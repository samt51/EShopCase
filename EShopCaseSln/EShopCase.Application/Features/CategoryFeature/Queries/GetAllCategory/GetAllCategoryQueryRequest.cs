using EShopCase.Application.Bases;
using MediatR;

namespace EShopCase.Application.Features.CategoryFeature.Queries.GetAllCategory;

public class GetAllCategoryQueryRequest :  IRequest<ResponseDto<PagedResult<GetAllCategoryQueryResponse>>>
{
    public string? Name { get; init; }   
    public string? SortBy { get; init; }   
    public string? SortDir { get; init; } 
    
    public int Page { get; init; } = 1;       
    public int PageSize { get; init; } = 20;   
}