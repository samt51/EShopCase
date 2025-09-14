using EShopCase.Application.Dtos.OrderItemsDto;
using EShopCase.Application.Dtos.UserDtos;

namespace EShopCase.Application.Features.OrderFeature.Queries.GetByOrder;

public class GetByOrderQueryResponse(
    int id,
    UserResponseDto userResponseDto,
    string address,
    List<OrderItemsRequestDto> orderItemsRequest)
{
    public int Id { get; set; } = id;
    public UserResponseDto UserResponseDto { get; set; } = userResponseDto;
    public string Address { get; set; } = address;
    public List<OrderItemsRequestDto> OrderItemsRequest { get; set; } = orderItemsRequest;
}