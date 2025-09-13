using EShopCase.Domain.Common;

namespace EShopCase.Domain.Entities;

public class OrderItem:EntityBase
{
    public int ProductId { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; }
}