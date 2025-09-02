using Ecommerce.Entities.DTO.Orders;
using FluentValidation;
using System;
using System.Linq;

namespace Ecommerce.API.Validators.Order
{
    public class UpdateOrderValidator : AbstractValidator<UpdateOrderRequest>
    {
        public UpdateOrderValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Invalid status. Must be Pending, Shipped, Delivered, or Cancelled.");
        }
    }
}