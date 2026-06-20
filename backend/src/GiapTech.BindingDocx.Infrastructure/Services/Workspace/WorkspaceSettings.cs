using GiapTech.BindingDocx.Application.Workspace.Interfaces;
using Microsoft.Extensions.Configuration;

namespace GiapTech.BindingDocx.Infrastructure.Services.Workspace;

public class WorkspaceSettings(IConfiguration configuration) : IWorkspaceSettings
{
    public string TemplatePath => configuration["Workspace:TemplatePath"] ?? "";
}
