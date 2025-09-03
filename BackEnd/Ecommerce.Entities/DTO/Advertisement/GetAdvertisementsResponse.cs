using Ecommerce.Utilities.Enums;
using System.Text.Json.Serialization;

namespace Ecommerce.Entities.DTO.Advertisement;
public class GetAdvertisementsResponse
{
    public Guid Id { get; set; }
    public string? ProductId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ImageOrientation ImageOrientation { get; set; }
    public string Image { get; set; }
}
