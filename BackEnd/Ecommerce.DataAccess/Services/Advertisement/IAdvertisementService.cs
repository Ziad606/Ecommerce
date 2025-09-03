using Ecommerce.Entities.DTO.Advertisement;
using Ecommerce.Entities.Shared.Bases;

namespace Ecommerce.DataAccess.Services.Advertisement;
public interface IAdvertisementService
{
    Task<Response<Guid>> CreateAdvertisementAsync(CreateAdvertisementRequest request, CancellationToken cancellationToken = default);
    Task<Response<GetAdvertisementsResponse>> GetAdvertisementByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Response<bool>> UpdateAdvertisementAsync(UpdateAdvertisementRequest request, CancellationToken cancellationToken = default);
    Task<Response<bool>> DeleteAdvertisementAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Response<List<GetAdvertisementsResponse>>> GetAllAdvertisementsAsync(CancellationToken cancellationToken = default);
}
