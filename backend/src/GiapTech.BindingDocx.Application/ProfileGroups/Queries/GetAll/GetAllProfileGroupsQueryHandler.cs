using GiapTech.BindingDocx.Application.ProfileGroups.DTOs;
using GiapTech.BindingDocx.Domain.Interfaces;
using MediatR;

namespace GiapTech.BindingDocx.Application.ProfileGroups.Queries.GetAll;

public class GetAllProfileGroupsQueryHandler(IProfileGroupRepository repository)
    : IRequestHandler<GetAllProfileGroupsQuery, IEnumerable<ProfileGroupDto>>
{
    public async Task<IEnumerable<ProfileGroupDto>> Handle(GetAllProfileGroupsQuery request, CancellationToken cancellationToken)
    {
        var groups = request.ActiveOnly
            ? await repository.GetActiveGroupsAsync(cancellationToken)
            : await repository.GetAllAsync(cancellationToken);

        return groups.Select(g => new ProfileGroupDto(
            g.Id,
            g.Name,
            g.Description,
            g.TemplatePath,
            g.SortOrder,
            g.IsActive,
            g.CreatedAt
        ));
    }
}
