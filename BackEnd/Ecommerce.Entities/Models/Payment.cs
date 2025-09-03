using Ecommerce.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.Models;
public sealed class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public Status Status { get; set; } = Status.Pending;
    public string StripePaymentIntentId { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // discount property
    public decimal OriginalAmount { get; set; } 
    public decimal? DiscountAmount { get; set; } 
    public string? PromoCodeUsed { get; set; } 
    public Guid? OrderId { get; set; }
    public Order? Order { get; set; }

    public string PaymentMethod { get; set; } = "card"; // card
    public string PaymentMethodDetails { get; set; } = string.Empty; // json for card details
}
