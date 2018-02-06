
namespace CountryInfo.Repository.Helpers
{
    using System.Collections.Generic;
    using System.Linq;

    public static class ListExtensions
    {
        public static void RemoveRange<T>(this List<T> source, IEnumerable<T> rangeToRemove)
        {
            if (rangeToRemove == null | !rangeToRemove.Any()) return;

            foreach (var item in rangeToRemove)
            {
                source.Remove(item);
            }
        }
    }
}