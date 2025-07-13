using HouseBroker.Application.Interface.DIRegistration;

namespace HouseBroker.Application.Interface.Services;

public interface IImageStorageService
{
    /// <summary>
    /// Saves the image bytes according to property and returns the accessible URL/path.
    /// </summary>
    /// <param name="fileContent">Image bytes</param>
    /// <param name="fileName">Original file name or desired file name</param>
    /// <returns>URL or path to the saved image</returns>
    Task<string> SaveImageAsync(Guid propertyId, string fileName, Stream fileStream);
}