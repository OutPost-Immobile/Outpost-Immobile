using Microsoft.EntityFrameworkCore;

namespace OutpostImmobile.Core.Common.Response;

/// <summary>
/// https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/sort-filter-page?view=aspnetcore-10.0
/// </summary>
/// <typeparam name="T"></typeparam>
public class PagedResponse<T> : List<T>
{
    public int PageIndex { get; private set; }
    public int TotalPages { get; private set; }
    
    public PagedResponse(List<T> items, int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);

        this.AddRange(items);
    }

    public bool HasPreviousPage => PageIndex > 1;

    public bool HasNextPage => PageIndex < TotalPages;

    public static async Task<PagedResponse<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
    {
        var count = await source.CountAsync();
        
        var items = await source
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return new PagedResponse<T>(items, count, pageIndex, pageSize);
    }
}