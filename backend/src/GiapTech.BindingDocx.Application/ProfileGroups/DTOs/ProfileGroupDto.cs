namespace GiapTech.BindingDocx.Application.ProfileGroups.DTOs;

public record ProfileGroupDto(
    Guid Id,
    string Name,
    string? Description,
    string? TemplatePath,
    int SortOrder,
    bool IsActive,
    DateTime CreatedAt
);
