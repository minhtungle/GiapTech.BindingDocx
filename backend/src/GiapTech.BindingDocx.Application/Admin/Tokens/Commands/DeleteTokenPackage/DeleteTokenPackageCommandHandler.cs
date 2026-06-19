using GiapTech.BindingDocx.Application.Common.Exceptions;
using GiapTech.BindingDocx.Domain.Interfaces;
using MediatR;

namespace GiapTech.BindingDocx.Application.Admin.Tokens.Commands.DeleteTokenPackage;

public class DeleteTokenPackageCommandHandler(ITokenRepository tokenRepository)
    : IRequestHandler<DeleteTokenPackageCommand>
{
    public async Task Handle(DeleteTokenPackageCommand request, CancellationToken cancellationToken)
    {
        var packages = await tokenRepository.GetAllPackagesAsync(cancellationToken);
        if (!packages.Any(p => p.Id == request.Id))
            throw new NotFoundException($"Token package {request.Id} not found.");

        await tokenRepository.DeletePackageAsync(request.Id, cancellationToken);
    }
}
