using CheckoutAssignment.Models;
using CheckoutAssignment.Storage;
using FluentValidation;

namespace CheckoutAssignment.Validation
{
    public class BasketValidator : AbstractValidator<Basket>
    {
        public BasketValidator(IApplicationStorage storage)
        {
            RuleFor(reg => reg.Owner).NotEmpty();
            RuleFor(reg => reg.Orders).SetCollectionValidator(new ItemOrderValidator(storage));
        }
    }
}
