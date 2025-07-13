using HouseBroker.Application.CommonDto;
using HouseBroker.Application.Interface;
using HouseBroker.Application.Interface.Messaging;
using HouseBroker.Application.Interface.Services;
using HouseBroker.Application.Property.Dto;

namespace HouseBroker.Application.Property.Command.UpdateProperty;

public class UpdatePropertyCommand : UpdatePropertyRequestDto, ICommand<Guid>
{
    
}

internal sealed class UpdatePropertyCommandHandler : ICommandHandler<UpdatePropertyCommand, Guid>
{
    private readonly IGenericRepository<Domain.Entities.Property> _propertyRepository;
    private readonly ILocationService _locationService;


    public UpdatePropertyCommandHandler(IGenericRepository<Domain.Entities.Property> propertyRepository, ILocationService locationService)
    {
        _propertyRepository = propertyRepository;
        _locationService = locationService;
    }

    public async Task<Response<Guid>> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.Id);
        if (property is null)
            return Response.Failure<Guid>(HttpContextError.Ambiguous("The country with the specified name already exist!"));
        
        // Resolve or create location
        var locationId = await _locationService.GetOrCreateLocationIdAsync(
            request.LocationId,
            request.CityId,
            request.Area,
            request.PostalCode,
            cancellationToken);
        
        // Update entity
        property.Price = request.Price;
        property.PropertyType = request.PropertyType;
        property.LocationId = locationId;
        property.IsSold = request.IsSold;
        property.Feature = request.Feature;

        _propertyRepository.UpdateAsync(property);
        return property.Id;
    }
}