using FluentValidation;

namespace GiapTech.BindingDocx.Application.Admin.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Password).MinimumLength(6).MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Password));
        RuleFor(x => x.Role).NotEmpty().Must(r => r == "admin" || r == "user")
            .WithMessage("Role must be 'admin' or 'user'.");
    }
}
