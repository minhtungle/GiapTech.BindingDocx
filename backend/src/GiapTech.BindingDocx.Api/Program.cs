using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using GiapTech.BindingDocx.Api.Extensions;
using GiapTech.BindingDocx.Api.Middleware;
using GiapTech.BindingDocx.Application;
using GiapTech.BindingDocx.Infrastructure;
using GiapTech.BindingDocx.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

// Resolve workspace template path (relative to ContentRoot or absolute)
var configuredTemplatePath = builder.Configuration["Workspace:TemplatePath"] ?? "../../export_file_temp";
var resolvedTemplatePath = Path.IsPathRooted(configuredTemplatePath)
    ? configuredTemplatePath
    : Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, configuredTemplatePath));
builder.Configuration["Workspace:TemplatePath"] = resolvedTemplatePath;

// Application + Infrastructure
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// CurrentUserService
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Controllers
builder.Services.AddControllers();

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is required.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// CORS
var corsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(corsOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

// Swagger
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigureOptions>();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Run migrations
using (var scope = app.Services.CreateScope())
{
    var migrator = scope.ServiceProvider.GetRequiredService<DatabaseMigrator>();
    await migrator.MigrateAsync();
}

// Middleware pipeline
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseSerilogRequestLogging();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Serve uploaded files from local storage
app.UseStaticFiles();

app.Run();
