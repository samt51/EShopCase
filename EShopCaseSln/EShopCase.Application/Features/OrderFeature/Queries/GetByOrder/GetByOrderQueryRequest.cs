using EShopCase.Application.Bases;
using MediatR;

namespace EShopCase.Application.Features.OrderFeature.Queries.GetByOrder;

public class GetByOrderQueryRequest : IRequest<ResponseDto<GetByOrderQueryResponse>>
{
    public int OrderId { get; set; }
    public int? UserId { get; set; }
}