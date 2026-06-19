using GiapTech.BindingDocx.Application.ProfileGroups.DTOs;
using MediatR;

namespace GiapTech.BindingDocx.Application.ProfileGroups.Queries.GetAll;

public record GetAllProfileGroupsQuery(bool ActiveOnly = true) : IRequest<IEnumerable<ProfileGroupDto>>;
