using Microsoft.Azure.Cosmos;

namespace vesa.Cosmos.Extensions;

public static class FeedIteratorExtensions
{
    public static async Task<IEnumerable<T>> ReadAllAsync<T>(this FeedIterator<T> feedIterator, CancellationToken cancellationToken = default)
    {
        var items = new List<T>();
        using (feedIterator)
        {
            while (feedIterator.HasMoreResults)
            {
                FeedResponse<T> response = await feedIterator.ReadNextAsync(cancellationToken);
                foreach (var item in response)
                {
                    items.Add(item);
                }
            }
        }
        return items;
    }
}
