using GiapTech.BindingDocx.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace GiapTech.BindingDocx.Infrastructure.Storage;

public class MinioStorageProvider(IConfiguration configuration, ILogger<MinioStorageProvider> logger) : IStorageProvider
{
    private readonly IMinioClient _minioClient = new MinioClient()
        .WithEndpoint(configuration["Storage:Minio:Endpoint"] ?? "localhost:9000")
        .WithCredentials(
            configuration["Storage:Minio:AccessKey"] ?? "minioadmin",
            configuration["Storage:Minio:SecretKey"] ?? "minioadmin")
        .WithSSL(bool.Parse(configuration["Storage:Minio:UseSSL"] ?? "false"))
        .Build();

    private readonly string _bucketName = configuration["Storage:Minio:BucketName"] ?? "bindingdocx";

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder, CancellationToken ct = default)
    {
        await EnsureBucketExistsAsync(ct);

        var safeFileName = Path.GetFileNameWithoutExtension(fileName)
            + "_" + DateTime.UtcNow.Ticks
            + Path.GetExtension(fileName);

        var objectName = $"{folder}/{safeFileName}";

        await _minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(GetContentType(fileName)), ct);

        logger.LogInformation("File saved to MinIO: {ObjectName}", objectName);
        return objectName;
    }

    public async Task<Stream> GetFileAsync(string filePath, CancellationToken ct = default)
    {
        var memoryStream = new MemoryStream();
        await _minioClient.GetObjectAsync(new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(filePath)
            .WithCallbackStream(s => s.CopyTo(memoryStream)), ct);
        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task DeleteFileAsync(string filePath, CancellationToken ct = default)
    {
        await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(filePath), ct);
    }

    public async Task<bool> FileExistsAsync(string filePath, CancellationToken ct = default)
    {
        try
        {
            await _minioClient.StatObjectAsync(new StatObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(filePath), ct);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public string GetFileUrl(string filePath)
    {
        var endpoint = configuration["Storage:Minio:Endpoint"] ?? "localhost:9000";
        return $"http://{endpoint}/{_bucketName}/{filePath}";
    }

    private async Task EnsureBucketExistsAsync(CancellationToken ct)
    {
        var exists = await _minioClient.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_bucketName), ct);
        if (!exists)
            await _minioClient.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(_bucketName), ct);
    }

    private static string GetContentType(string fileName)
    {
        return Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".pdf" => "application/pdf",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            _ => "application/octet-stream"
        };
    }
}
