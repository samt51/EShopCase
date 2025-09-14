using EShopCase.Application.Bases;
using EShopCase.Application.Interfaces.Mapper;
using EShopCase.Application.Interfaces.UnitOfWorks;
using EShopCase.Domain.Entities;
using MediatR;

namespace EShopCase.Application.Features.UserFeature.Queries.GetByUser;

public class GetByUserQueryHandler(IMapper mapper, IUnitOfWork unitOfWork) : BaseHandler(mapper, unitOfWork),
    IRequestHandler<GetByUserQueryRequest, ResponseDto<GetByUserQueryResponse>>
{
    public async Task<ResponseDto<GetByUserQueryResponse>> Handle(GetByUserQueryRequest request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.GetReadRepository<Users>().GetAsync(x=>x.Id == request.Id&&!x.IsDeleted);
        
        var rsp = new GetByUserQueryResponse(data.Id,data.FirstName + " " + data.LastName, data.Email);

        return new ResponseDto<GetByUserQueryResponse>().Success(rsp);
    }
}