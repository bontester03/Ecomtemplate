using KhanHomeFloralLine.Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace KhanHomeFloralLine.Infrastructure.Storage;

public class LocalBlobStorageService(IConfiguration configuration) : IBlobStorageService
{
    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var folder = configuration["Storage:LocalPath"] ?? "uploads";
        Directory.CreateDirectory(folder);

        var safeFileName = $"{Guid.NewGuid()}-{fileName}";
        var fullPath = Path.Combine(folder, safeFileName);

        await using var file = File.Create(fullPath);
        await stream.CopyToAsync(file, cancellationToken);

        return $"/{folder.Replace("\\", "/")}/{safeFileName}";
    }
}

