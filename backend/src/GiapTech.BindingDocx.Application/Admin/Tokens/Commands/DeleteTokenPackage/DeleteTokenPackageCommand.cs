using MediatR;

namespace GiapTech.BindingDocx.Application.Admin.Tokens.Commands.DeleteTokenPackage;

public record DeleteTokenPackageCommand(Guid Id) : IRequest;
