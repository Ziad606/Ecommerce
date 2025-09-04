using Ecommerce.Entities.DTO.PromoCodes;
using Ecommerce.Entities.Shared.Bases;

namespace Ecommerce.DataAccess.Services.PromoCode;
public interface IPromoCodeService
{
    Task<Response<string>> CreatePromoCodeAsync(CreatePromoCodeRequest request, CancellationToken cancellationToken = default);

    Task<Response<GetPromoCodeResponse>> GetPromoCodeByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<Response<List<GetPromoCodeResponse>>> GetAllPromoCodesAsync(CancellationToken cancellationToken = default);

    Task<Response<bool>> UpdatePromoCodeAsync(UpdatePromoCodeRequest request, CancellationToken cancellationToken = default);

    Task<Response<bool>> DeletePromoCodeAsync(string code, CancellationToken cancellationToken = default);
}
