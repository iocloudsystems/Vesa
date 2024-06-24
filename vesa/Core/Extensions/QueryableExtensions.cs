namespace vesa.Core.Extensions;

public static class QueryableExtensions
{
    public static async Task<List<TSource>> ToListAsync<TSource>
    (
       this IQueryable<TSource> source,
       CancellationToken cancellationToken = default
    )
    {
        var list = new List<TSource>();
        await foreach (var element in source.AsAsyncEnumerable().WithCancellation(cancellationToken))
        {
            list.Add(element);
        }

        return list;
    }

    public static IAsyncEnumerable<TSource> AsAsyncEnumerable<TSource>(this IQueryable<TSource> source)
    {
        if (source is IAsyncEnumerable<TSource> asyncEnumerable)
        {
            return asyncEnumerable;
        }

        throw new InvalidOperationException(nameof(TSource));
    }
}
