using GiapTech.BindingDocx.Application.Workspace.DTOs;
using MediatR;

namespace GiapTech.BindingDocx.Application.Workspace.Queries.GetGroupKeys;

public record GetGroupKeysQuery(string GroupId) : IRequest<GroupKeysDto>;
