using System.Linq.Expressions;
using HouseBroker.Application.Interface;
using HouseBroker.Domain.Interfaces;
using HouseBroker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HouseBroker.Infrastructure.Services;

public class GenericRepository<T> : IGenericRepository<T>
    where T : class
{

    private readonly HouseBrokerDbContext _dbContext;
    private readonly DbSet<T> _dbSet;
    
    public GenericRepository(HouseBrokerDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }

    
    public async Task<T> GetByIdAsync(Guid? id)
    {
        return await _dbSet.FindAsync(id);
    }

    public IQueryable<T> GetAll()
    {
        return _dbSet.AsQueryable();
    }

    public IQueryable<T> GetAllNoTracking()
    {
        return _dbSet.AsQueryable().AsNoTracking();
    }

    public async Task InsertAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;    }

    public void UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
    }

    public async Task AddRangeAsync(IEnumerable<T> entity)
    {
        await _dbSet.AddRangeAsync(entity);
    }

    public Task RemoveRange(IEnumerable<T> entity)
    {
        _dbSet.RemoveRange(entity);
        return Task.CompletedTask;
    }

    public async Task<Guid> InsertAndGetAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _dbContext.SaveChangesAsync();

        //Returns primaryKey value
        return (Guid)entity.GetType().GetProperty("Id").GetValue(entity, null);
    }

    public async Task<Guid> UpdateAndGetAsync(T entity, CancellationToken cancellationToken)
    {
        _dbSet.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        //Returns primaryKey value
        return (Guid)entity.GetType().GetProperty("Id").GetValue(entity, null);
    }

    public Task UpdateRangeAsync(IEnumerable<T> entity)
    {
        _dbSet.UpdateRange(entity);
        return Task.CompletedTask;
    }
}