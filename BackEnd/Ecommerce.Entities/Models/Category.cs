using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Entities.Models
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

    }
}
