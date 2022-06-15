using System;
using System.Collections.Generic;

namespace TestTask.Extensions
{
    /// <summary>
    /// Метод расширение для IList
    /// </summary>
    public static class IListExtension
    {
        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The System.Predicate`1 delegate that defines the conditions of the elements to remove.</param>
        /// <returns>The number of elements removed from the System.Collections.Generic.List`1.</returns>
        /// <exception cref="ArgumentNullException">T:System.ArgumentNullException:match is null.</exception>
        public static int RemoveAll<T>(this IList<T> list, Predicate<T> match)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }

            int removedCount = 0;

            for (int i = list.Count - 1; i >= 0 ; i--)
            {
                if (match(list[i]))
                {
                    list.RemoveAt(i);
                    removedCount++;
                }
            }

            return removedCount;
        }
    }
}
