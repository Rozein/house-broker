using HouseBroker.Application.Interface.DIRegistration;

namespace HouseBroker.Application.Interface.Services;

public interface ILocationService: IScopedDependency
{
    Task<Guid> GetOrCreateLocationIdAsync(Guid? locationId, Guid cityId, string area, string postalCode, CancellationToken cancellationToken);

}