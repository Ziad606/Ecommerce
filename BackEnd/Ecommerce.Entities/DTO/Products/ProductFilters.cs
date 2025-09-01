using System.Text.Json.Serialization;
using Ecommerce.Utilities.Enums;

namespace Ecommerce.Entities.DTO.Shared.Product;

public class ProductFilters<TSortColumn> : RequestFilters<TSortColumn>
    where TSortColumn : struct, Enum
{
    public bool Status { get; set; } = true;
}