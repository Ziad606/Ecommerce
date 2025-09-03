using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.DataAccess.Services.ImageUploading;
using Ecommerce.DataAccess.Services.Products;
using Ecommerce.Entities.DTO.Advertisement;
using Ecommerce.Entities.Shared.Bases;
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
                ImageLink = image
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


}
