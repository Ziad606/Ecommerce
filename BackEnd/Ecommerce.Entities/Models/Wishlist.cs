using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ecommerce.Entities.Models.Auth.Identity;


namespace Ecommerce.Entities.Models
{
    public class Wishlist
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string BuyerId { get; set; }

        [ForeignKey(nameof(BuyerId))]
        public User Buyer { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public List<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    }
}
