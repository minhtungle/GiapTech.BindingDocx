namespace GiapTech.BindingDocx.Application.Workspace.DTOs;

public record WorkspaceGroupDto(
    string Id,
    string Name,
    bool IsAvailable,
    int FileCount
);
