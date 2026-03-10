namespace KhanHomeFloralLine.Application.Abstractions;

public interface IBlobStorageService
{
    Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default);
}

