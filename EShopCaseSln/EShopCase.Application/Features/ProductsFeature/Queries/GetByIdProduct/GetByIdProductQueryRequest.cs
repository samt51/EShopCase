using EShopCase.Application.Bases;
using MediatR;

namespace EShopCase.Application.Features.ProductsFeature.Queries.GetByIdProduct;

public class GetByIdProductQueryRequest : IRequest<ResponseDto<GetByIdProductQueryResponse>>
{
    public GetByIdProductQueryRequest(int id)
    {
        this.Id = id;   
    }
    public int Id { get; set; }
}