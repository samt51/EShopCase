using EShopCase.Application.Bases;
using EShopCase.Application.Interfaces.Mapper;
using EShopCase.Application.Interfaces.UnitOfWorks;
using EShopCase.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopCase.Application.Features.OrderFeature.Queries.GetByOrder;

public class GetByOrderQueryHandler(IMapper mapper, IUnitOfWork unitOfWork) : BaseHandler(mapper, unitOfWork),
    IRequestHandler<GetByOrderQueryRequest, ResponseDto<GetByOrderQueryResponse>>
{
    public async Task<ResponseDto<GetByOrderQueryResponse>> Handle(GetByOrderQueryRequest request, CancellationToken cancellationToken)
    {
        var order = await unitOfWork.GetReadRepository<Order>().GetAsync(x=>x.Id==request.OrderId&&!x.IsDeleted,
            include:x=>x.Include(y=>y.Items));

        var map = mapper.Map<GetByOrderQueryResponse,Order>(order);

        return new ResponseDto<GetByOrderQueryResponse>().Success(map);
    }
}