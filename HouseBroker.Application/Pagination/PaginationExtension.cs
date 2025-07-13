using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HouseBroker.Application.Pagination;

public static class PaginationExtension
{
    public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(this IQueryable<TDestination> queryable, int pageNumber, int pageSize) where TDestination : class
        => PaginatedList<TDestination>.CreateAsync(queryable.AsNoTracking(), pageNumber, pageSize);
    
    public static Task<PaginatedList<TDestination>> EnumerablePaginatedListAsync<TDestination>(this IEnumerable<TDestination> enumerable, int pageNumber, int pageSize, int count) where TDestination : class
        => PaginatedList<TDestination>.CreateEnumerablePaginationAsync(enumerable, pageNumber, pageSize, count);
    

}
