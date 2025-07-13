using HouseBroker.Application;
using HouseBroker.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HouseBroker.Infrastructure.Services;

public class UnitOfWorkService(HouseBrokerDbContext dbContext, IMediator mediator) : IUnitOfWorkService
{
    private bool _disposed = false;
    private readonly IMediator _mediator = mediator;

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            Rollback();
            throw;
        }    
    }

    public void Rollback()
    {
        foreach (var entry in dbContext.ChangeTracker.Entries())
        {
            switch (entry.State)  
            {  
                case EntityState.Modified:  
                    entry.State = EntityState.Unchanged;  
                    break;  
                case EntityState.Added:  
                    entry.State = EntityState.Detached;  
                    break;  
                case EntityState.Deleted:  
                    entry.Reload();  
                    break;  
                default: break;  
            }  
        }
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                dbContext.DisposeAsync();
            }
        }
        _disposed = true;
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}