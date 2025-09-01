using System.Text.Json.Serialization;
using Ecommerce.Utilities.Enums;
using Newtonsoft.Json.Converters;
namespace Ecommerce.Entities.DTO.Shared;
public class RequestFilters<TSortColumn>
    where TSortColumn : struct, Enum
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchValue { get; init; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TSortColumn? SortColumn { get; set; }
    
    [JsonConverter(typeof(StringEnumConverter))]
    public SortDirection? SortDirection { get; init; } = Utilities.Enums.SortDirection.ASC;
}

