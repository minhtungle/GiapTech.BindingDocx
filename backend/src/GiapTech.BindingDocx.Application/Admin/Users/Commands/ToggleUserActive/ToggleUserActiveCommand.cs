using MediatR;

namespace GiapTech.BindingDocx.Application.Admin.Users.Commands.ToggleUserActive;

public record ToggleUserActiveCommand(Guid Id) : IRequest<bool>;
