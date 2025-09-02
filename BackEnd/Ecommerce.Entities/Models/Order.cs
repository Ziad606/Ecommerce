using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ecommerce.Entities.Models.Auth.Identity;
using Ecommerce.Utilities.Enums;

namespace Ecommerce.Entities.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string BuyerId { get; set; } = string.Empty;

        [ForeignKey(nameof(BuyerId))]
        public User Buyer { get; set; } = default!;

        // [Required]
        // public string SellerId { get; set; }

        // [ForeignKey(nameof(SellerId))]
        // public Seller Seller { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public decimal TotalPrice { get; set; }

        public string ShippingAddress { get; set; }= string.Empty;
        public decimal ShippingPrice { get; set; }
        public string TrackingNumber { get; set; } = string.Empty;
                                                  
        public string CourierService { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public DateTime? ShippedDate { get; set; }

        public DateTime? DeliveredDate { get; set; }

        public DateTime? CancelledDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public List<OrderItem> OrderItems { get; set; } = [];

        public ICollection<Payment> Payments { get; set; } = [];
    }
}
