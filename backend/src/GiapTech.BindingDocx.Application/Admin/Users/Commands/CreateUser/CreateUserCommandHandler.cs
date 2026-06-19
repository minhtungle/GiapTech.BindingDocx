using GiapTech.BindingDocx.Application.Common.Exceptions;
using GiapTech.BindingDocx.Domain.Entities;
using GiapTech.BindingDocx.Domain.Interfaces;
using MediatR;

namespace GiapTech.BindingDocx.Application.Admin.Users.Commands.CreateUser;

public class CreateUserCommandHandler(IUserRepository userRepository, ITokenRepository tokenRepository)
    : IRequestHandler<CreateUserCommand, Guid>
{
    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await userRepository.UsernameExistsAsync(request.Username, null, cancellationToken))
            throw new ValidationException(new Dictionary<string, string[]>
                { ["username"] = ["Username already exists."] });

        if (await userRepository.EmailExistsAsync(request.Email, null, cancellationToken))
            throw new ValidationException(new Dictionary<string, string[]>
                { ["email"] = ["Email already exists."] });

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12),
            Role = request.Role,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        var id = await userRepository.CreateAsync(user, cancellationToken);
        await tokenRepository.AddTokensAsync(id, 0, "Account created", null, cancellationToken);
        return id;
    }
}
