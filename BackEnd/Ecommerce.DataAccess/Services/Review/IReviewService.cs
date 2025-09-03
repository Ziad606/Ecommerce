using Ecommerce.Entities.DTO.Reviews;
using Ecommerce.Entities.Shared;
using Ecommerce.Entities.Shared.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DataAccess.Services.Review
{
    public interface IReviewService
    {
        Task<Response<Guid>> CreateReviewAsync(CreateReviewRequest dto, string buyerId);

        Task<Response<GetReviewsResponse>> UpdateReviewAsync(Guid id, UpdateReviewRequest dto, string buyerId);

        Task<Response<string>> DeleteReviewAsync(Guid id, DeleteReviewRequest dto, string buyerId);

        Task<Response<PaginatedList<GetReviewsResponse>>> GetAllReviewsAsync(GetReviewsRequest dto);

        Task<Response<GetReviewsResponse>> GetReviewByIdAsync(Guid id);

    }
}