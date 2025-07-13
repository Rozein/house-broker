using HouseBroker.Application.Interface.DIRegistration;

namespace HouseBroker.Application;

public interface IUnitOfWorkService : IDisposable,IScopedDependency
{
    Task CommitAsync(CancellationToken cancellationToken);
    void Rollback();
}