using GiapTech.BindingDocx.Application.Common.Exceptions;
using GiapTech.BindingDocx.Domain.Interfaces;
using MediatR;

namespace GiapTech.BindingDocx.Application.Admin.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler(IUserRepository userRepository)
    : IRequestHandler<UpdateUserCommand>
{
    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"User {request.Id} not found.");

        if (await userRepository.UsernameExistsAsync(request.Username, request.Id, cancellationToken))
            throw new ValidationException(new Dictionary<string, string[]>
                { ["username"] = ["Username already exists."] });

        if (await userRepository.EmailExistsAsync(request.Email, request.Id, cancellationToken))
            throw new ValidationException(new Dictionary<string, string[]>
                { ["email"] = ["Email already exists."] });

        user.Username = request.Username;
        user.Email = request.Email;
        user.Role = request.Role;
        user.IsActive = request.IsActive;

        if (!string.IsNullOrEmpty(request.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);

        await userRepository.UpdateProfileAsync(user, cancellationToken);
    }
}
