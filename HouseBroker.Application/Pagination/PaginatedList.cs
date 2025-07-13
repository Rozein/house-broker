using HouseBroker.Domain.Utils;
using Microsoft.EntityFrameworkCore;

namespace HouseBroker.Application.Pagination;

public class PaginatedList<T>
{
    public IReadOnlyCollection<T> Items { get; }
    private int PageNumber { get; }
    private int TotalPages { get; }
    public int TotalCount { get; }

    private PaginatedList(IReadOnlyCollection<T> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Items = items;
    }
    
    public static PaginatedList<T> Empty(int pageNumber = HouseBrokerConstants.PaginationDefaults.PageNumber, int pageSize = HouseBrokerConstants.PaginationDefaults.PageSize) 
        => new (Array.Empty<T>(), 0, pageNumber, pageSize);

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;
    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = (await source.ToListAsync()).Count;
        
        var items = await (NoPagination(pageNumber, pageSize)
            ? source
            : source.Skip((pageNumber - 1) * pageSize).Take(pageSize)).ToListAsync();

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
    
    public static Task<PaginatedList<T>> CreateEnumerablePaginationAsync(IEnumerable<T> source, int pageNumber, int pageSize, int count)
    {
        var items = (NoPagination(pageNumber, pageSize)
            ? source
            : source.Skip((pageNumber - 1) * pageSize).Take(pageSize)).ToList();

        return Task.FromResult(new PaginatedList<T>(items, count, pageNumber, pageSize));
    }

    private static bool NoPagination(int pageNumber, int pageSize) => pageNumber == 0 && pageSize == 0; 
    
    public static Task<PaginatedList<T>> PaginationWrapperAsync(List<T> source, int pageNumber, int pageSize, int count)
    {
        return Task.FromResult(new PaginatedList<T>(source, count, pageNumber, pageSize));
    }
}
