using EShopCase.Application.Bases;
using EShopCase.Application.Interfaces.Mapper;
using EShopCase.Application.Interfaces.UnitOfWorks;
using EShopCase.Domain.Entities;
using MediatR;

namespace EShopCase.Application.Features.UserFeature.Queries.GetAllUser;

public class GetAllUserQueryHandler(IMapper mapper, IUnitOfWork unitOfWork) : BaseHandler(mapper, unitOfWork),
    IRequestHandler<GetAllUserQueryRequest, ResponseDto<List<GetAllUserQueryResponse>>>
{
    public async Task<ResponseDto<List<GetAllUserQueryResponse>>> Handle(GetAllUserQueryRequest request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.GetReadRepository<Users>().GetAllQueryAsync(x=>!x.IsDeleted,cancellationToken:cancellationToken);

        var result = data.Select(x =>
            new GetAllUserQueryResponse( x.Id, x.Email, x.FirstName + ' ' + x.LastName)).ToList();

        return new ResponseDto<List<GetAllUserQueryResponse>>().Success(result);

    }
}