using Ecommerce.Entities.DTO.Shared;
using Ecommerce.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Reviews
{
    public class GetReviewsRequest : RequestFilters<ReviewSorting>
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }

    }
}
