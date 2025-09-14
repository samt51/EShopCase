using EShopCase.Application.Bases;
using MediatR;

namespace EShopCase.Application.Features.OrderFeature.Queries.GetAllOrder;

public class GetAllOrderQueryRequest : IRequest<ResponseDto<List<GetAllOrderQueryResponse>>>
{
    public int UserId { get; set; }
}