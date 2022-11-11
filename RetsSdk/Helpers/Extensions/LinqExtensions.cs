namespace CrestApps.RetsSdk.Helpers.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    public static class LinqExtensions
    {
        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> items, int partitionSize)
        {
            return items.Select((item, index) => new { item, index })
                           .GroupBy(x => x.index / partitionSize)
                           .Select(g => g.Select(x => x.item));
        }
    }
}
