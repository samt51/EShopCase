using EShopCase.Application.Bases;
using MediatR;

namespace EShopCase.Application.Features.UserFeature.Queries.GetByUser;

public class GetByUserQueryRequest(int id) : IRequest<ResponseDto<GetByUserQueryResponse>>
{
    public int Id { get; set; } = id;
}