using EShopCase.Application.Dtos.OrderItemsDto;

namespace EShopCase.Application.Features.OrderFeature.Queries.GetAllOrder;

public class GetAllOrderQueryResponse(int buyerUserId, string address, List<OrderItemsRequestDto> orderItemsRequest)
{
    public int BuyerUserId { get; set; } = buyerUserId;
    public string Address { get; set; } = address;
    public List<OrderItemsRequestDto> OrderItemsRequest { get; set; } = orderItemsRequest;
}