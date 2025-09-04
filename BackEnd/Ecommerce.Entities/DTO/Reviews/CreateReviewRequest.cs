using Ecommerce.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Reviews
{
    public class CreateReviewRequest
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public ReviewRating Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
