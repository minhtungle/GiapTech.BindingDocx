using GiapTech.BindingDocx.Application.Workspace.Interfaces;
using GiapTech.BindingDocx.Domain.Interfaces;
using GiapTech.BindingDocx.Infrastructure.Auth;
using GiapTech.BindingDocx.Infrastructure.Data;
using GiapTech.BindingDocx.Infrastructure.Repositories;
using GiapTech.BindingDocx.Infrastructure.Services.Workspace;
using GiapTech.BindingDocx.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GiapTech.BindingDocx.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<DatabaseMigrator>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProfileGroupRepository, ProfileGroupRepository>();
        services.AddScoped<ITemplateFileRepository, TemplateFileRepository>();
        services.AddScoped<IImportRepository, ImportRepository>();
        services.AddScoped<ITokenRepository, TokenRepository>();

        services.AddScoped<IJwtService, JwtService>();

        var storageProvider = configuration["Storage:Provider"] ?? "local";
        if (storageProvider.Equals("minio", StringComparison.OrdinalIgnoreCase))
            services.AddSingleton<IStorageProvider, MinioStorageProvider>();
        else
            services.AddSingleton<IStorageProvider, LocalStorageProvider>();

        // Workspace services
        services.AddSingleton<IWorkspaceSettings, WorkspaceSettings>();
        services.AddSingleton<ITemplateKeyExtractor, TemplateKeyExtractor>();
        services.AddSingleton<IExcelTemplateGenerator, ExcelTemplateGenerator>();
        services.AddSingleton<IExcelDataParser, ExcelDataParser>();
        services.AddSingleton<ITemplateRenderer, TemplateRenderer>();

        return services;
    }
}
