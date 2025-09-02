using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Entities.Models.Reviews;
using Ecommerce.Entities.Models;
using Ecommerce.Utilities.Enums;

namespace Ecommerce.Entities.DTO.Product
{
    public class GetProductResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Dimensions { get; set; }
        public string Material { get; set; }
        public string SKU { get; set; }
        public int StockQuantity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public List<string> ImageUrls { get; set; }
        public double? AverageRating { get; set; }
        public int ReviewCount { get; set; }

    }
}
