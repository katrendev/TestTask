using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask
{
    public static class Extensions
    {
        /// <summary>
        /// Значение с индексом
        /// </summary>
        /// <typeparam name="T">Тип хранящегося значения.</typeparam>
        private class IndexedValue<T>
        {
            public IndexedValue(T value, int index)
            {
                Value = value;
                Index = index;
            }
            public T Value { get; set; }
            public int Index { get; set; }
        }

        /// <summary>
        /// Ф-ция расшерения, удаляет все элементы из списка, по условию.
        /// </summary>
        /// <typeparam name="T">Тип содержимого списка.</typeparam>
        /// <param name="list">Список.</param>
        /// <param name="func">Условие удаления элемента.</param>
        /// <returns>Тот же list, с удаленными элементами.</returns>
        public static IList<T> RemoveAll<T>(this IList<T> list, Func<T, bool> func)
        {
            var indexesForRemove = list
                .Select((x, index) => new IndexedValue<T>(x, index))
                .Where(x => func(x.Value))
                .Select(x => x.Index)
                .ToArray();
            int countRemoveItems = 0;
            foreach (int index in indexesForRemove)
            {
                list.RemoveAt(index - countRemoveItems);
                countRemoveItems++;
            }
            return list;
        }
    }
}
