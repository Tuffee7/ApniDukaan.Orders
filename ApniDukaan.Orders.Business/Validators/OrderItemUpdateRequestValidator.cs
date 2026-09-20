namespace ApniDukaan.Orders.Business.Validators
{
    using System;
    using FluentValidation;
    using ApniDukaan.Orders.Business.RequestDTO;

    public class OrderItemUpdateRequestValidator : AbstractValidator<OrderItemUpdateRequest>
    {
        public OrderItemUpdateRequestValidator()
        {
            RuleFor(x => x.ProductID)
                .NotEqual(Guid.Empty).WithMessage("ProductID is required.");

            RuleFor(x => x.UnitPrice)
                .NotEmpty().WithErrorCode("UnitPrice is required.")
                .GreaterThanOrEqualTo(0).WithMessage("UnitPrice must be >= 0.");

            RuleFor(x => x.Quantity)
                .NotEmpty().WithErrorCode("Quantity is required.")
                .GreaterThan(0).WithMessage("Quantity must be > 0.");
        }
    }
}
