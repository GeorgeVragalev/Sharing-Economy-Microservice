using FluentValidation;
using OrderAPI.Models;

namespace OrderAPI.Validations;

public class OrderValidation : AbstractValidator<PlaceOrderRequestModel>
{
    public OrderValidation()
    {
        RuleFor(x => x.ItemId).NotEmpty().WithMessage("You must supply the item id that you want to order");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("We need to know who is ordering");
        RuleFor(x => x.ReservationPeriod).NotEmpty().WithMessage("We need to know for how long you want to order");
    }
}