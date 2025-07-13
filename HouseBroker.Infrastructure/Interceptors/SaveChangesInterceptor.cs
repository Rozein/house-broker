using HouseBroker.Application.CurrentUserService;
using HouseBroker.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HouseBroker.Infrastructure.Interceptors
{
    //Notes :You don't necessarily have to use EF interceptors.
    //You can achieve the same behavior by overriding the SaveChangesAsync method on the DbContext and adding your custom logic there.
    public class SaveChangesInterceptor : Microsoft.EntityFrameworkCore.Diagnostics.SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUserService;
        public SaveChangesInterceptor(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            UpdateEntities(eventData);
            return base.SavingChanges(eventData, result);
        }


        private void UpdateEntities(DbContextEventData eventData)
        {
            var dbContext = eventData.Context!;
            foreach (var entry in dbContext.ChangeTracker.Entries()
                         .Where(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
            {
                if (entry.Entity is IAuditableEntity auditable)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditable.CreationTime = DateTime.UtcNow;
                            auditable.CreatedBy = _currentUserService.UserId;
                            break;
                        case EntityState.Modified:
                            auditable.LastModifiedTime = DateTime.UtcNow;
                            auditable.LastModifiedBy = _currentUserService.UserId;
                            break;
                    }
                }

                if (entry is { Entity: ISoftDeleteEntity softDelete, State: EntityState.Deleted })
                {
                    softDelete.IsDeleted = true;
                    softDelete.DeletionTime = DateTime.UtcNow;
                    softDelete.DeletedBy = _currentUserService.UserId;
                    entry.State = EntityState.Modified;
                }
            }


        }
    }
}
