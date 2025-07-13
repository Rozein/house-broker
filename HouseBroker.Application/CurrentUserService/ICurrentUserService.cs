using HouseBroker.Application.Interface.DIRegistration;

namespace HouseBroker.Application.CurrentUserService;

public interface ICurrentUserService: IScopedDependency
{
    Guid UserId { get;}
    string? UserRole{ get;}
}