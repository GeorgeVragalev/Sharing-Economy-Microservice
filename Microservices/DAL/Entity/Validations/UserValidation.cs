using FluentValidation;

namespace DAL.Entity.Validations;

public class UserValidation : AbstractValidator<User>
{
    public UserValidation()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("You idiot, give a name");
        RuleFor(x => x.Phone).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}