using GiapTech.BindingDocx.Application.Tokens.DTOs;
using MediatR;

namespace GiapTech.BindingDocx.Application.Tokens.Queries.GetPackages;

public record GetTokenPackagesQuery : IRequest<IEnumerable<TokenPackageDto>>;
