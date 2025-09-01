namespace Ecommerce.Entities.DTO.Orders
{
    public class OrderItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}