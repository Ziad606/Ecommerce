using Ecommerce.Entities.DTO.Advertisement;
using Ecommerce.Entities.Shared.Bases;

namespace Ecommerce.DataAccess.Services.Advertisement;
public interface IAdvertisementService
{
    Task<Response<Guid>> CreateAdvertisementAsync(CreateAdvertisementRequest request, CancellationToken cancellationToken = default);
}
