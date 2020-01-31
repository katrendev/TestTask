using System;
using System.Collections.Generic;

namespace TestTask
{
	public static class IListExtensions
	{
		/// <summary>
		/// Удаляет все элементы коллекции, совпавшие по предикату
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="match">Делегат, определяющий условия удаления элемента.</param>
		/// <returns></returns>
		public static int RemoveAll<T>(this IList<T> list, Predicate<T> match)
		{
			var count = 0;
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (match(list[i]))
				{
					list.RemoveAt(i);
					count++;
				}
			}

			return count;
		}
	}
}