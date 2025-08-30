using Ecommerce.Entities.Models.Auth.Identity;

namespace Ecommerce.Entities.Models.Reviews
{
    public class Review
    {
        public Guid Id { get; set; }
        public string BuyerId { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }

        public double Rating { get; set; } // Overall Rating = average of Attributes
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;


        // Navigation Properties
        public User Buyer { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }


    }
}
