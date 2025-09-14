using EShopCase.Application.Bases;
using EShopCase.Application.Interfaces.Mapper;
using EShopCase.Application.Interfaces.UnitOfWorks;
using EShopCase.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopCase.Application.Features.OrderFeature.Commands.CreateOrder;

public class CreateOrderCommandHandler(IMapper mapper, IUnitOfWork unitOfWork) : BaseHandler(mapper, unitOfWork),
    IRequestHandler<CreateOrderCommandRequest, ResponseDto<CreateOrderCommandResponse>>
{
    public async Task<ResponseDto<CreateOrderCommandResponse>> Handle(CreateOrderCommandRequest request,
        CancellationToken cancellationToken)
    {

        if (request.OrderItemsRequest is null || request.OrderItemsRequest.Count == 0)
            throw new ArgumentException("OrderItemsRequest boş olamaz.", nameof(request.OrderItemsRequest));


        var buyer = await unitOfWork.GetReadRepository<Users>()
            .GetAsync(x => x.Id == request.BuyerUserId && !x.IsDeleted);
        if (buyer is null)
            throw new InvalidOperationException($"Buyer bulunamadı: {request.BuyerUserId}");


        var ids = request.OrderItemsRequest
            .Select(i => i.ProductId)
            .Where(id => id > 0)
            .Distinct()
            .ToArray();

        if (ids.Length == 0)
            throw new ArgumentException("Geçerli en az bir ProductId gönderilmelidir.",
                nameof(request.OrderItemsRequest));


        var productsQ = await unitOfWork.GetReadRepository<Products>()
            .GetAllQueryAsync(p => !p.IsDeleted && ids.Contains(p.Id),
                include: q => q.Include(p => p.Category),cancellationToken:cancellationToken);

        var products = await productsQ
            .Select(p => new { p.Id, p.Price })
            .ToListAsync(cancellationToken);

        var existingIds = products.Select(p => p.Id).ToHashSet();
        var missing = ids.Where(id => !existingIds.Contains(id)).ToList();
        if (missing.Count > 0)
            throw new InvalidOperationException($"Geçersiz ProductId(ler): {string.Join(", ", missing)}");

        var order = mapper.Map<Order, CreateOrderCommandRequest>(request);


        var productPriceLookup = products.ToDictionary(p => p.Id, p => p.Price);
        var orderItems = request.OrderItemsRequest.Select(i => new OrderItem
        {
            ProductId = i.ProductId,
            Quantity = i.Quantity,
            Price = productPriceLookup[i.ProductId],
        }).ToList();

        await unitOfWork.OpenTransactionAsync(cancellationToken);
        try
        {
            var savedOrder = await unitOfWork.GetWriteRepository<Order>()
                .AddAsync(order, cancellationToken);

            await unitOfWork.SaveAsync(cancellationToken);


            foreach (var item in orderItems)
                item.OrderId = savedOrder.Id;

     
            await unitOfWork.GetWriteRepository<OrderItem>()
                .AddRangeAsync(orderItems);

            await unitOfWork.SaveAsync(cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);

            var resp = new CreateOrderCommandResponse { OrderId = savedOrder.Id };
            return new ResponseDto<CreateOrderCommandResponse>().Success(resp);
        }
        catch
        {
            await unitOfWork.RollBackAsync(cancellationToken);
            throw;
        }
    }
}