using System.Runtime.CompilerServices;
using HouseBroker.Application.CurrentUserService;
using HouseBroker.Application.Interface;
using HouseBroker.Application.Interface.Messaging;
using HouseBroker.Application.Interface.Services;
using HouseBroker.Application.Property.Dto;
using HouseBroker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HouseBroker.Application.Features.Property.Command.CreateProperty;
public class CreatePropertyCommand :CreatePropertyRequestDto, ICommand<Guid>
{
}

/// <summary>
/// Handles the creation of a Property entity, including resolving or creating its associated Location.
/// </summary>
internal sealed class CreatePropertyCommandHandler :ICommandHandler<CreatePropertyCommand,Guid>
{
    private readonly IGenericRepository<Domain.Entities.Property> _propertyRepository;
    private readonly IGenericRepository<Location> _locationRepository;
    private readonly ILocationService _locationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IGenericRepository<Broker>  _brokerRepository;
    private readonly ILogger<CreatePropertyCommandHandler> _logger;

    public CreatePropertyCommandHandler(IGenericRepository<Domain.Entities.Property> propertyRepository, IGenericRepository<Location> locationRepository, ILocationService locationService, ICurrentUserService currentUserService, IGenericRepository<Broker> brokerRepository, ILogger<CreatePropertyCommandHandler> logger)
    {
        _propertyRepository = propertyRepository;
        _locationRepository = locationRepository;
        _locationService = locationService;
        _currentUserService = currentUserService;
        _brokerRepository = brokerRepository;
        _logger = logger;
    }

    public async Task<Response<Guid>> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        #region Location Insert
        _logger.LogDebug("Resolving location for CityId: {CityId}, Area: {Area}, PostalCode: {PostalCode}", 
            request.CityId, request.Area, request.PostalCode);
        //location Insert
        var locationId = await _locationService.GetOrCreateLocationIdAsync(
            request.LocationId,
            request.CityId,
            request.Area,
            request.PostalCode,
            cancellationToken
        );
        _logger.LogDebug("Location resolved successfully with LocationId: {LocationId}", locationId);

        #endregion

        var brokerId = await _brokerRepository.GetAllNoTracking()
            .Where(x => x.UserId == _currentUserService.UserId).Select(x => x.Id).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        // Create a new Property entity
        var propertyEntity = new Domain.Entities.Property
        {
            Price = request.Price,
            PropertyType = request.PropertyType,
            LocationId = locationId,
            BrokerId = brokerId,
            IsSold = false,
            Feature = request.Feature
        };
        await _propertyRepository.InsertAsync(propertyEntity);
        _logger.LogInformation("Property created successfully with Id: {PropertyId} for user {UserId}", 
            propertyEntity.Id, userId);

        return propertyEntity.Id;
    }
}