using EShopCase.Domain.Common;

namespace EShopCase.Domain.Entities;

public class Order : EntityBase
{
    public Order()
    {
        
    }

    public Order(int buyerUserId,string address)
    {
        this.BuyerUserId = buyerUserId;
        this.Address = address; 
    }
    public int BuyerUserId { get; set; }
    public Users Users { get; set; }
    public string Address { get; set; }
    public IList<OrderItem> Items { get; set; }
    
    public decimal GetTotalPrice => Items.Sum(x => x.Price);

}