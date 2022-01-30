using System.Collections.Generic;
using System.Linq;

namespace Magnetar
{
    public static class IEnumerableExtensions
    {
        public static T Random<T>(this IEnumerable<T> collection)
        {
            int count = collection.Count();
            if(count == 0)
            {
                return default;
            }

            return collection.ElementAt(UnityEngine.Random.Range(0, count));
        }
    }
}
