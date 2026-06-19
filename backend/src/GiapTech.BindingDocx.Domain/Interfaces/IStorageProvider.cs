namespace GiapTech.BindingDocx.Domain.Interfaces;

public interface IStorageProvider
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder, CancellationToken ct = default);
    Task<Stream> GetFileAsync(string filePath, CancellationToken ct = default);
    Task DeleteFileAsync(string filePath, CancellationToken ct = default);
    Task<bool> FileExistsAsync(string filePath, CancellationToken ct = default);
    string GetFileUrl(string filePath);
}
