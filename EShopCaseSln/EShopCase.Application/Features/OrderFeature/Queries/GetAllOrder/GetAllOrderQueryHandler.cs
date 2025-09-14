using EShopCase.Application.Bases;
using EShopCase.Application.Dtos.OrderItemsDto;
using EShopCase.Application.Interfaces.Mapper;
using EShopCase.Application.Interfaces.UnitOfWorks;
using EShopCase.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopCase.Application.Features.OrderFeature.Queries.GetAllOrder;

public class GetAllOrderQueryHandler(IMapper mapper, IUnitOfWork unitOfWork) : BaseHandler(mapper, unitOfWork),
    IRequestHandler<GetAllOrderQueryRequest, ResponseDto<List<GetAllOrderQueryResponse>>>
{
    public async Task<ResponseDto<List<GetAllOrderQueryResponse>>> Handle(GetAllOrderQueryRequest request,
        CancellationToken cancellationToken)
    {

             await unitOfWork.GetReadRepository<Users>()
            .GetAsync(x => x.Id == request.UserId && !x.IsDeleted);

        var ordersQ = await unitOfWork.GetReadRepository<Order>().GetAllQueryAsync(
            predicate: x => x.BuyerUserId == request.UserId && !x.IsDeleted,
            include: q => q.Include(o => o.Items),
            cancellationToken: cancellationToken);

        var orderResponse = await ordersQ
            .OrderByDescending(o => o.CreatedDate)
            .Select(o => new GetAllOrderQueryResponse(
                o.BuyerUserId,
                o.Address,
                o.Items
                    .Select(i => new OrderItemsRequestDto
                    {
                        ProductId = i.ProductId,
                        Price     = i.Price,
                        Quantity  = i.Quantity,
                        OrderId   = i.OrderId
                    })
                    .ToList()
            ))
            .ToListAsync(cancellationToken);

        return new ResponseDto<List<GetAllOrderQueryResponse>>().Success(orderResponse);
    }
}