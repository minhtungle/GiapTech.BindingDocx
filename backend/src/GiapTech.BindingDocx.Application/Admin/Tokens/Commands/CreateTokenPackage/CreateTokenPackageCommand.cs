using MediatR;

namespace GiapTech.BindingDocx.Application.Admin.Tokens.Commands.CreateTokenPackage;

public record CreateTokenPackageCommand(
    string Name,
    int TokenAmount,
    decimal PricePerToken,
    decimal TotalPrice,
    bool IsActive,
    int SortOrder
) : IRequest<Guid>;
