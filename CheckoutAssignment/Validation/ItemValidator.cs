using CheckoutAssignment.Models;
using CheckoutAssignment.Storage;
using FluentValidation;

namespace CheckoutAssignment.Validation
{
    public class ItemValidator : AbstractValidator<Item>
    {
        public ItemValidator(IApplicationStorage storage)
        {
            RuleFor(reg => reg.Name).NotEmpty().WithMessage("Name must be present");
            RuleFor(reg => reg.Price).NotEmpty().ExclusiveBetween(0, float.MaxValue).WithMessage("Price must be positive");
        }
    }
}
