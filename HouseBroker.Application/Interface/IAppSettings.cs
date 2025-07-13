using HouseBroker.Application.Interface.DIRegistration;

namespace HouseBroker.Application.Interface;

public interface IAppSettings: ISingletonDependency
{
    string ImageBaseUrl { get; }

}