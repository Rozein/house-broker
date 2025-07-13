using HouseBroker.Application.Interface;
using HouseBroker.Application.Interface.Services;
using HouseBroker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HouseBroker.Infrastructure.Services;

public class LocationService(IGenericRepository<Location> locationRepository,ILogger<LocationService> logger) : ILocationService
{
    // Determine the LocationId for the property.
    // Assumption:
    // - If LocationId is provided in the request, use it directly.
    // - If not provided, we try to find an existing Location by matching CityId, Area, and PostalCode.
    // - If a matching Location exists, we reuse it to avoid duplication.
    // - If no match is found, we create a new Location record and use its Id.
    // This approach ensures consistent reuse of location data while allowing flexibility in input.
    public  async Task<Guid> GetOrCreateLocationIdAsync(Guid? locationId, Guid cityId, string area, string postalCode,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting location resolution process");

        if (locationId.HasValue && locationId != Guid.Empty)
            return locationId.Value;

        var existingLocation = await locationRepository.GetAllNoTracking()
            .FirstOrDefaultAsync(l =>
                    l.CityId == cityId &&
                    l.Area == area &&
                    l.PostalCode == postalCode,
                cancellationToken);

        if (existingLocation != null)
            return existingLocation.Id;

        var newLocation = new Location
        {
            CityId = cityId,
            Area = area,
            PostalCode = postalCode
        };

        await locationRepository.InsertAsync(newLocation);
        return newLocation.Id;    
    }
}