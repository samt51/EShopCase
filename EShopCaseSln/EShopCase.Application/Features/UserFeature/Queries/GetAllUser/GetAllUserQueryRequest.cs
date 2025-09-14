using EShopCase.Application.Bases;
using MediatR;

namespace EShopCase.Application.Features.UserFeature.Queries.GetAllUser;

public class GetAllUserQueryRequest : IRequest<ResponseDto<List<GetAllUserQueryResponse>>>
{
    
}