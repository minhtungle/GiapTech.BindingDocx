using GiapTech.BindingDocx.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GiapTech.BindingDocx.Infrastructure.Storage;

public class LocalStorageProvider(IConfiguration configuration, ILogger<LocalStorageProvider> logger) : IStorageProvider
{
    private readonly string _basePath = configuration["Storage:LocalPath"]
        ?? Path.Combine(Directory.GetCurrentDirectory(), "storage");

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder, CancellationToken ct = default)
    {
        var folderPath = Path.Combine(_basePath, folder);
        Directory.CreateDirectory(folderPath);

        var safeFileName = Path.GetFileNameWithoutExtension(fileName)
            + "_" + DateTime.UtcNow.Ticks
            + Path.GetExtension(fileName);

        var fullPath = Path.Combine(folderPath, safeFileName);

        using var fileOutput = File.Create(fullPath);
        await fileStream.CopyToAsync(fileOutput, ct);

        logger.LogInformation("File saved: {Path}", fullPath);

        var relativePath = Path.Combine(folder, safeFileName).Replace('\\', '/');
        return relativePath;
    }

    public async Task<Stream> GetFileAsync(string filePath, CancellationToken ct = default)
    {
        var fullPath = Path.Combine(_basePath, filePath.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"File not found: {filePath}");

        var memoryStream = new MemoryStream();
        using var fileStream = File.OpenRead(fullPath);
        await fileStream.CopyToAsync(memoryStream, ct);
        memoryStream.Position = 0;
        return memoryStream;
    }

    public Task DeleteFileAsync(string filePath, CancellationToken ct = default)
    {
        var fullPath = Path.Combine(_basePath, filePath.Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            logger.LogInformation("File deleted: {Path}", fullPath);
        }
        return Task.CompletedTask;
    }

    public Task<bool> FileExistsAsync(string filePath, CancellationToken ct = default)
    {
        var fullPath = Path.Combine(_basePath, filePath.Replace('/', Path.DirectorySeparatorChar));
        return Task.FromResult(File.Exists(fullPath));
    }

    public string GetFileUrl(string filePath) => $"/files/{filePath.Replace('\\', '/')}";
}
