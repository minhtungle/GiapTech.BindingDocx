using GiapTech.BindingDocx.Application.Common.Exceptions;
using GiapTech.BindingDocx.Domain.Interfaces;
using MediatR;

namespace GiapTech.BindingDocx.Application.Admin.Tokens.Commands.UpdateTokenPackage;

public class UpdateTokenPackageCommandHandler(ITokenRepository tokenRepository)
    : IRequestHandler<UpdateTokenPackageCommand>
{
    public async Task Handle(UpdateTokenPackageCommand request, CancellationToken cancellationToken)
    {
        var packages = await tokenRepository.GetAllPackagesAsync(cancellationToken);
        var package = packages.FirstOrDefault(p => p.Id == request.Id)
            ?? throw new NotFoundException($"Token package {request.Id} not found.");

        package.Name = request.Name;
        package.TokenAmount = request.TokenAmount;
        package.PricePerToken = request.PricePerToken;
        package.TotalPrice = request.TotalPrice;
        package.IsActive = request.IsActive;
        package.SortOrder = request.SortOrder;

        await tokenRepository.UpdatePackageAsync(package, cancellationToken);
    }
}
