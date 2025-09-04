using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.Entities.DTO.PromoCodes;
using Ecommerce.Entities.Shared.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.DataAccess.Services.PromoCode;
public class PromoCodeService(
    AuthContext context,
    ResponseHandler responseHandler,
    ILogger<PromoCodeService> logger
    ) : IPromoCodeService
{
    private readonly AuthContext _context = context;
    private readonly ResponseHandler _responseHandler = responseHandler;
    private readonly ILogger<PromoCodeService> _logger = logger;


    public async Task<Response<string>> CreatePromoCodeAsync(CreatePromoCodeRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (await _context.PromoCodes.AnyAsync(p => p.Code == request.Code, cancellationToken))
            {
                return _responseHandler.BadRequest<string>("Promo code already exists.");
            }

            var promo = new Entities.Models.PromoCode
            {
                Code = request.Code,
                StartAt = request.StartAt,
                EndAt = request.EndAt,
                DiscountPercentage = request.DiscountPercentage,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.PromoCodes.AddAsync(promo, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Promo code {Code} created successfully", promo.Code);
            return _responseHandler.Created(promo.Code, "Promo code created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating promo code {Code}", request.Code);
            return _responseHandler.InternalServerError<string>("An error occurred while creating the promo code.");
        }
    }

    public async Task<Response<GetPromoCodeResponse>> GetPromoCodeByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        try
        {
            var promo = await _context.PromoCodes.FindAsync(new object[] { code }, cancellationToken);
            if (promo == null)
            {
                return _responseHandler.NotFound<GetPromoCodeResponse>("Promo code not found.");
            }

            return _responseHandler.Success(new GetPromoCodeResponse(promo), "promo code was fetched successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving promo code {Code}", code);
            return _responseHandler.InternalServerError<GetPromoCodeResponse>("An error occurred while retrieving the promo code.");
        }
    }

    public async Task<Response<List<GetPromoCodeResponse>>> GetAllPromoCodesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var promos = await _context.PromoCodes
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);

            return _responseHandler.Success(promos.Select(p => new GetPromoCodeResponse(p)).ToList(), "promo codes was fetched successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all promo codes");
            return _responseHandler.InternalServerError<List<GetPromoCodeResponse>>("An error occurred while retrieving promo codes.");
        }
    }

    public async Task<Response<bool>> UpdatePromoCodeAsync(UpdatePromoCodeRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var promo = await _context.PromoCodes.FindAsync(new object[] { request.Code }, cancellationToken);
            if (promo == null)
            {
                return _responseHandler.NotFound<bool>("Promo code not found.");
            }

            promo.StartAt = request.StartAt ?? promo.StartAt;
            promo.EndAt = request.EndAt ?? promo.EndAt;
            promo.DiscountPercentage = request.DiscountPercentage ?? promo.DiscountPercentage;
            promo.IsDeleted = request.IsDeleted ?? promo.IsDeleted;
            promo.UpdatedAt = DateTime.UtcNow;

            _context.PromoCodes.Update(promo);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Promo code {Code} updated successfully", promo.Code);
            return _responseHandler.Success(true, "Promo code updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating promo code {Code}", request.Code);
            return _responseHandler.InternalServerError<bool>("An error occurred while updating the promo code.");
        }
    }

    public async Task<Response<bool>> DeletePromoCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        try
        {
            var promo = await _context.PromoCodes.FindAsync(new object[] { code }, cancellationToken);
            if (promo == null)
            {
                return _responseHandler.NotFound<bool>("Promo code not found.");
            }

            promo.IsDeleted = true;
            promo.UpdatedAt = DateTime.UtcNow;

            _context.PromoCodes.Update(promo);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Promo code {Code} marked as deleted", code);
            return _responseHandler.Success(true, "Promo code deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting promo code {Code}", code);
            return _responseHandler.InternalServerError<bool>("An error occurred while deleting the promo code.");
        }
    }
}
