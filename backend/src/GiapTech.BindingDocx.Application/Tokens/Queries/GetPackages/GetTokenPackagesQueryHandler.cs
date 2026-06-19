using GiapTech.BindingDocx.Application.Tokens.DTOs;
using GiapTech.BindingDocx.Domain.Interfaces;
using MediatR;

namespace GiapTech.BindingDocx.Application.Tokens.Queries.GetPackages;

public class GetTokenPackagesQueryHandler(ITokenRepository tokenRepository)
    : IRequestHandler<GetTokenPackagesQuery, IEnumerable<TokenPackageDto>>
{
    public async Task<IEnumerable<TokenPackageDto>> Handle(GetTokenPackagesQuery request, CancellationToken cancellationToken)
    {
        var packages = await tokenRepository.GetActivePackagesAsync(cancellationToken);

        return packages.Select(p => new TokenPackageDto(
            p.Id,
            p.Name,
            p.TokenAmount,
            p.PricePerToken,
            p.TotalPrice,
            p.IsActive,
            p.SortOrder
        ));
    }
}
