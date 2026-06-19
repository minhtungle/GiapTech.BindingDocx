using GiapTech.BindingDocx.Domain.Entities;
using GiapTech.BindingDocx.Domain.Interfaces;
using MediatR;

namespace GiapTech.BindingDocx.Application.Admin.Tokens.Commands.CreateTokenPackage;

public class CreateTokenPackageCommandHandler(ITokenRepository tokenRepository)
    : IRequestHandler<CreateTokenPackageCommand, Guid>
{
    public async Task<Guid> Handle(CreateTokenPackageCommand request, CancellationToken cancellationToken)
    {
        var package = new TokenPackage
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            TokenAmount = request.TokenAmount,
            PricePerToken = request.PricePerToken,
            TotalPrice = request.TotalPrice,
            IsActive = request.IsActive,
            SortOrder = request.SortOrder,
            CreatedAt = DateTime.UtcNow,
        };
        return await tokenRepository.CreatePackageAsync(package, cancellationToken);
    }
}
