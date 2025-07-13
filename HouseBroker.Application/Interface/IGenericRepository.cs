using HouseBroker.Application.Interface.DIRegistration;
using HouseBroker.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HouseBroker.Application.Interface;

public interface IGenericRepository<T> where T : class
{
    Task<T> GetByIdAsync(Guid? id);
    IQueryable<T> GetAll();
    IQueryable<T> GetAllNoTracking();
    Task InsertAsync(T entity);  // Return Task instead of Task<EntityEntry<T>>
    Task DeleteAsync(T entity);
    void UpdateAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entity);
    Task RemoveRange(IEnumerable<T> entity);
    Task<Guid> InsertAndGetAsync(T entity);
    Task<Guid> UpdateAndGetAsync(T entity, CancellationToken cancellationToken);
    Task UpdateRangeAsync(IEnumerable<T> entity);
}