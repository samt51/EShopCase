namespace EShopCase.Application.Dtos.OrderItemsDto;

public class OrderItemsRequestDto
{
    public int Id { get; set; }
    public int ProductId { get;  set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int OrderId { get; set; }
}