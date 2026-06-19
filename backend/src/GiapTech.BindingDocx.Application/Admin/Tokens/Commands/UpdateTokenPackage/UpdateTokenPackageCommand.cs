using MediatR;

namespace GiapTech.BindingDocx.Application.Admin.Tokens.Commands.UpdateTokenPackage;

public record UpdateTokenPackageCommand(
    Guid Id,
    string Name,
    int TokenAmount,
    decimal PricePerToken,
    decimal TotalPrice,
    bool IsActive,
    int SortOrder
) : IRequest;
