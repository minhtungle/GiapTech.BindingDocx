using MediatR;

namespace GiapTech.BindingDocx.Application.Admin.Users.Commands.AdjustUserTokens;

public record AdjustUserTokensCommand(
    Guid UserId,
    int Amount,
    string Description
) : IRequest;
