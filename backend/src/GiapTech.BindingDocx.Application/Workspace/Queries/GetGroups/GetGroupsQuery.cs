using GiapTech.BindingDocx.Application.Workspace.DTOs;
using MediatR;

namespace GiapTech.BindingDocx.Application.Workspace.Queries.GetGroups;

public record GetGroupsQuery : IRequest<IEnumerable<WorkspaceGroupDto>>;
