using Ecommerce.Entities.Models.Reviews;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Entities.Models.Auth.Identity
{
    public class User : IdentityUser
    {
        // Personal Information
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
    
        // Contact & Address
        public string DefaultShippingAddress { get; set; }
        public string DefaultBillingAddress { get; set; }
 
        // Account Settings
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLoginDate { get; set; }
  
        // Navigation Properties
        public Cart Cart { get; set; }
        public Wishlist Wishlist { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}
