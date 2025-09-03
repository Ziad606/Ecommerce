using Ecommerce.Utilities.Enums;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Ecommerce.Entities.DTO.Advertisement;
public class UpdateAdvertisementRequest
{
    public Guid Id { get; set; }
    public Guid? ProductId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ImageOrientation? ImageOrientation { get; set; }
    public IFormFile? Image { get; set; }
}
