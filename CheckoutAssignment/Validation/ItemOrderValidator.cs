using CheckoutAssignment.Models;
using CheckoutAssignment.Storage;
using FluentValidation;

namespace CheckoutAssignment.Validation
{
    public class ItemOrderValidator : AbstractValidator<ItemOrder>
    {
        public ItemOrderValidator(IApplicationStorage storage)
        {
            RuleFor(order => order.Amount).Must(amount => amount > 0);
            RuleFor(order => order.Item).NotNull();
            RuleFor(order => order.Item).Must(item => storage.ContainsLineItem(item.Id) && item.Equals(storage.GetLineItem(item.Id)));
        }
    }
}
