using FluentValidation;
using OrderAPI.Models;

namespace OrderAPI.Validations;

public class OrderValidation : AbstractValidator<OrderModel>
{
    public OrderValidation()
    {
        // RuleFor(x => x.Name).NotEmpty().WithMessage("You idiot, give a name");
    }
}