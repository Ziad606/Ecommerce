namespace Ecommerce.Entities.DTO.Orders
{
    public class GetOrdersResponse
    {
        public List<OrderDetailsResponse> Orders { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool IsEmpty => Orders.Count == 0;
    }
}
