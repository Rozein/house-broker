using HouseBroker.Application.Interface.Services;

namespace HouseBroker.Infrastructure.Services;

public class LocalImageStorageService : IImageStorageService
{
    private readonly string _basePath;
    private readonly string _baseUrl;

    public LocalImageStorageService(string basePath, string baseUrl)
    {
        _basePath = basePath;
        _baseUrl = baseUrl;
    }

    public async Task<string> SaveImageAsync(Guid propertyId, string fileName, Stream fileStream)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"); // e.g., 20250713143045123
        var safeFileName = Path.GetFileNameWithoutExtension(fileName); // Removes path & extension
        var extension = Path.GetExtension(fileName);                   // e.g., ".jpg"
        var uniqueFileName = $"{timestamp}_{safeFileName}{extension}";
        
        var propertyFolder = Path.Combine(_basePath, propertyId.ToString());
        if (!Directory.Exists(propertyFolder))
            Directory.CreateDirectory(propertyFolder);

        var filePath = Path.Combine(propertyFolder, uniqueFileName);

        await using var fileStreamDest = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(fileStreamDest);

        // Return URL for accessing image (depends on your hosting setup)
        return $"{propertyId}/{uniqueFileName}";
    }
}
