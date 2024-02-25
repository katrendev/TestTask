using System;
using System.Linq;
using System.Collections.Generic;

namespace TestTask.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Расширение для типа данных ICollection(IList)
        /// </summary>
        /// <returns>Коллекция без элементов, удаленных по условию</returns>
        /// <param name="this">Коллекция, с которой проводится работа</param>
        /// <param name="predicate">Условие удаления элементов из коллекции</param>
        public static void RemoveAll<T>(this ICollection<T> @this, Func<T, bool> predicate)
        {
            if (@this is List<T> list)
            {
                list.RemoveAll(new Predicate<T>(predicate));
            }
            else
            {
                List<T> itemsToDelete = @this
                    .Where(predicate)
                    .ToList();

                foreach (var item in itemsToDelete)
                {
                    @this.Remove(item);
                }
            }
        }
    }
}
