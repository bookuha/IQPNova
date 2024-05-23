using Microsoft.EntityFrameworkCore;

namespace IQP.Shared;

public class PagedList<T>
{
    public PagedList(IEnumerable<T> items, int page, int pageSize, int totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public IEnumerable<T> Items { get; set; }

    public int Page { get; }

    public int PageSize { get; }

    public int TotalCount { get; }

    public bool HasNextPage => Page * PageSize < TotalCount;

    public bool HasPreviousPage => Page > 1;
    

    public static async Task<PagedList<T>> CreateFromQueryAsync(IQueryable<T> query, int? page, int? pageSize)
    {
        var totalCount = await query.CountAsync();
        if (page != null && pageSize != null)
        {
            var items = await query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value).ToListAsync();
            return new(items, page.Value, pageSize.Value, totalCount);
        }

        return new(await query.ToListAsync(), 0, 0, totalCount);
    }
    
}

public static class PagedListMappingExtensions
{
    public static PagedList<U> Map<T, U>(this PagedList<T> source, Func<T, U> mapper)
    {
        var mappedItems = source.Items.Select(mapper);
        return new PagedList<U>(mappedItems, source.Page, source.PageSize, source.TotalCount);
    }
    
}