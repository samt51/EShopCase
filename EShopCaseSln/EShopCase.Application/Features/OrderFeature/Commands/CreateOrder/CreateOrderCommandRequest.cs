using EShopCase.Application.Bases;
using EShopCase.Application.Dtos.OrderItemsDto;
using MediatR;

namespace EShopCase.Application.Features.OrderFeature.Commands.CreateOrder;

public class CreateOrderCommandRequest(int buyerUserId, string address, List<OrderItemsRequestDto> orderItemsRequest)
    : IRequest<ResponseDto<CreateOrderCommandResponse>>
{
    public int BuyerUserId { get; set; } = buyerUserId;
    public string Address { get; set; } = address;
    public List<OrderItemsRequestDto> OrderItemsRequest { get; set; } = orderItemsRequest;
}