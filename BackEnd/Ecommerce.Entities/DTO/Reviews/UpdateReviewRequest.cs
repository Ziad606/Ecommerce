using Ecommerce.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Reviews
{
    public class UpdateReviewRequest
    {
        public ReviewRating Rating { get; set; }
        public string Comment { get; set; }
        public bool Confirm { get; set; }
    }
}
