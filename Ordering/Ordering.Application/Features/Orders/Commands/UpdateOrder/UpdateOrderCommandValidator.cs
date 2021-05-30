using FluentValidation;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommandValidator: AbstractValidator<UpdateOrderCommand>
    {
        public UpdateOrderCommandValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .NotNull().MaximumLength(50).WithMessage("Username must not exceed 50 characters");
            RuleFor(x => x.EmailAddress)
                .NotEmpty().WithMessage("Email address is required");
            RuleFor(x => x.TotalPrice)
                .NotEmpty().WithMessage("Total price is required")
                .GreaterThan(0).WithMessage("Total price must be greater than 0");
        }
    }
}