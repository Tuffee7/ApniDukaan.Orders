namespace ApniDukaan.Orders.Business.Validators
{
    using System;
    using FluentValidation;
    using ApniDukaan.Orders.Business.RequestDTO;

    public class OrderUpdateRequestValidator : AbstractValidator<OrderUpdateRequest>
    {
        public OrderUpdateRequestValidator()
        {
            RuleFor(x => x.OrderID)
                .NotEqual(Guid.Empty).WithMessage("OrderID is required.");

            RuleFor(x => x.UserID)
                .NotEqual(Guid.Empty).WithMessage("UserID is required.");

            RuleFor(x => x.OrderDate)
                .Must(d => d != default(DateTime)).WithMessage("OrderDate is required.")
                .Must(d => d <= DateTime.UtcNow).WithMessage("OrderDate cannot be in the future.");

            RuleFor(x => x.OrderItems)
                .NotNull().WithMessage("OrderItems cannot be null.")
                .Must(list => list.Count > 0).WithMessage("At least one order item is required.");
        }
    }
}
