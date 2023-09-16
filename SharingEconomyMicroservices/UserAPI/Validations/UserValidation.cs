using FluentValidation;
using UserAPI.Models;

namespace UserAPI.Validations;

public class UserValidation : AbstractValidator<UserModel>
{
    public UserValidation()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("You idiot, give a name");
        RuleFor(x => x.Phone).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}