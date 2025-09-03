using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.DataAccess.Services.ImageUploading;
using Ecommerce.DataAccess.Services.Products;
using Ecommerce.Entities.DTO.Advertisement;
using Ecommerce.Entities.Shared.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.DataAccess.Services.Advertisement;
public class AdvertisementService(
    AuthContext context,
    ResponseHandler responseHandler,
    ILogger<ProductService> logger,
    IImageUploadService imageUploadService
    ) : IAdvertisementService
{
    private readonly AuthContext _context = context;
    private readonly ResponseHandler _responseHandler = responseHandler;
    private readonly ILogger<ProductService> _logger = logger;
    private readonly IImageUploadService _imageUploadService = imageUploadService;

    public async Task<Response<Guid>> CreateAdvertisementAsync(CreateAdvertisementRequest request, CancellationToken cancellationToken = default)
    {

        try
        {
            var existingProduct = await _context.Products.FindAsync(request.ProductId, cancellationToken);
            if (existingProduct is null)
            {
                return _responseHandler.NotFound<Guid>("Product not found.");
            }

            var image = await _imageUploadService.UploadAsync(request.Image);

            var advertisement = new Entities.Models.Advertisement
            {
                ProductId = request.ProductId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                ImageOrientation = request.ImageOrientation,
                ImageLink = image,
                CreatedAt = DateTime.UtcNow,
            };

            await _context.Advertisements.AddAsync(advertisement, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Advertisement created with ID: {AdvertisementId}", advertisement.Id);
            return _responseHandler.Created<Guid>(advertisement.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating advertisement");
            return _responseHandler.InternalServerError<Guid>("An error occurred while creating the advertisement.");
        }
    }
    public async Task<Response<List<GetAdvertisementsResponse>>> GetAllAdvertisementsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var advertisements = await _context.Advertisements
                .Include(a => a.Product)
                .OrderByDescending(a => a.Id)
                .Select(a => new GetAdvertisementsResponse
                {
                    Id = a.Id,
                    ProductId = a.ProductId.ToString(),
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                    ImageOrientation = a.ImageOrientation,
                    Image = a.ImageLink
                })
                .ToListAsync(cancellationToken);



            _logger.LogInformation("Retrieved {Count} advertisements", advertisements.Count);
            return _responseHandler.Success(advertisements, "Advertisements was fetched successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving advertisements");
            return _responseHandler.InternalServerError<List<GetAdvertisementsResponse>>("An error occurred while retrieving advertisements.");
        }
    }

    public async Task<Response<GetAdvertisementsResponse>> GetAdvertisementByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var advertisement = await _context.Advertisements
                .Include(a => a.Product)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);


            if (advertisement == null)
            {
                _logger.LogWarning("Advertisement with ID {AdvertisementId} not found", id);
                return _responseHandler.NotFound<GetAdvertisementsResponse>("Advertisement not found.");
            }

            var response = new GetAdvertisementsResponse
            {
                Id = advertisement.Id,
                ProductId = advertisement.ProductId.ToString(),
                StartDate = advertisement.StartDate,
                EndDate = advertisement.EndDate,
                ImageOrientation = advertisement.ImageOrientation,
                Image = advertisement.ImageLink
            };

            _logger.LogInformation("Retrieved advertisement with ID: {AdvertisementId}", id);
            return _responseHandler.Success(response, "Advertisement was fetched successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving advertisement with ID: {AdvertisementId}", id);
            return _responseHandler.InternalServerError<GetAdvertisementsResponse>("An error occurred while retrieving the advertisement.");
        }
    }


    public async Task<Response<bool>> UpdateAdvertisementAsync(UpdateAdvertisementRequest request, CancellationToken cancellationToken = default)
    {

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var advertisement = await _context.Advertisements.FirstOrDefaultAsync(a => a.Id == request.Id && !a.IsDeleted, cancellationToken);
            if (advertisement == null)
            {
                _logger.LogWarning("Advertisement with ID {AdvertisementId} not found or deleted", request.Id);
                return _responseHandler.NotFound<bool>("Advertisement not found.");
            }

            if (request.ProductId is not null)
            {

                var existingProduct = await _context.Products.FindAsync(request.ProductId, cancellationToken);

                if (existingProduct is null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found for advertisement update", request.ProductId);
                    return _responseHandler.NotFound<bool>("Product not found.");
                }
            }
            advertisement.ProductId = request.ProductId ?? advertisement.ProductId;
            advertisement.StartDate = request.StartDate ?? advertisement.StartDate;
            advertisement.EndDate = request.EndDate ?? advertisement.EndDate;
            advertisement.ImageOrientation = request.ImageOrientation ?? advertisement.ImageOrientation;
            advertisement.UpdatedAt = DateTime.UtcNow;

            if (request.Image != null)
            {
                await _imageUploadService.DeleteAsync(advertisement.ImageLink);
                advertisement.ImageLink = await _imageUploadService.UploadAsync(request.Image);
            }

            _context.Advertisements.Update(advertisement);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("Advertisement with ID {AdvertisementId} updated", request.Id);
            return _responseHandler.Success(true, "Advertisement was updated successfullty");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error updating advertisement with ID: {AdvertisementId}", request.Id);
            return _responseHandler.InternalServerError<bool>("An error occurred while updating the advertisement.");
        }
    }

    public async Task<Response<bool>> DeleteAdvertisementAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var advertisement = await _context.Advertisements.FirstOrDefaultAsync(a => a.Id == id && a.IsDeleted, cancellationToken);
            if (advertisement == null)
            {
                _logger.LogWarning("Advertisement with ID {AdvertisementId} not found or already deleted", id);
                return _responseHandler.NotFound<bool>("Advertisement not found.");
            }


            advertisement.IsDeleted = false;
            advertisement.DeletedAt = DateTime.UtcNow;

            _context.Advertisements.Update(advertisement);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Advertisement with ID {AdvertisementId} deleted", id);
            return _responseHandler.Success(true, "Advertisement was deleted successfullty");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error deleting advertisement with ID: {AdvertisementId}", id);
            return _responseHandler.InternalServerError<bool>("An error occurred while deleting the advertisement.");
        }
    }




}
