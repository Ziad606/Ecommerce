using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Entities.Models
{
    public class WishlistItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); // عشان اول اما اعمل منه instance يعملو by default

        [Required]
        public Guid WishlistId { get; set; }

        [ForeignKey(nameof(WishlistId))]
        public Wishlist Wishlist { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
