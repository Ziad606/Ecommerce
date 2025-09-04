using Ecommerce.Entities.DTO.Profile;
using Ecommerce.Entities.Shared.Bases;

namespace Ecommerce.DataAccess.Services.Profile;
public interface IProfileService
{
    Task<Response<List<GetProfileAdminListResponse>>> GetAllProfilesAsync(CancellationToken cancellationToken = default);
    Task<Response<GetProfileAdminResponse>> GetProfileAdminByIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<Response<GetProfileResponse>> GetProfileByIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<Response<bool>> UpdateProfileAsync(string userId, UpdateProfileRequest request, CancellationToken cancellationToken = default);

}
