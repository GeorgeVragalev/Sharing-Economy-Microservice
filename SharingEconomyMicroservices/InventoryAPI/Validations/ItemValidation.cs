using FluentValidation;
using InventoryAPI.Models;

namespace InventoryAPI.Validations;

public class ItemValidation : AbstractValidator<ItemModel>
{
    public ItemValidation()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("You idiot, give a name");
    }
}