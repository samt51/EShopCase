using EShopCase.Application.Bases;
using MediatR;

namespace EShopCase.Application.Features.CategoryFeature.Queries.GetByIdCategory;

public class GetByIdCategoryQueryRequest(int id) : IRequest<ResponseDto<GetByIdCategoryQueryResponse>>
{
    public int Id { get; set; } = id;
}