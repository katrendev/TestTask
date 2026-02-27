using System;
using System.Collections.Generic;
using System.Linq;
using TestTask.Enums;
using TestTask.Helpers;
using TestTask.Streams;
using TestTask.Streams.Interfaces;
using TestTask.Structs;

namespace TestTask
{
	/// <summary>
	/// Корневой класс программы.
	/// </summary>
	public class Program
	{
		/// <summary>
		/// Программа принимает на входе 2 пути до файлов.
		/// Анализирует в первом файле кол-во вхождений каждой буквы (регистрозависимо). Например А, б, Б, Г и т.д.
		/// Анализирует во втором файле кол-во вхождений парных букв (не регистрозависимо). Например АА, Оо, еЕ, тт и т.д.
		/// По окончанию работы - выводит данную статистику на экран.
		/// </summary>
		/// <param name="args">Первый параметр - путь до первого файла.
		/// Второй параметр - путь до второго файла.</param>
		private static void Main(string[] args)
		{
			try
			{
				if (args.Length != 2)
				{
					throw new ArgumentException("Входных аргументов должно быть два ", nameof(args));
				}

				Dictionary<char, LetterStats> singleLetterStats = null;
				Dictionary<char, LetterStats> doubleLetterStats = null;

				using (IReadOnlyStream inputStream1 = new ReadOnlyStream(args[0]))
				{
					singleLetterStats = FillLetterStatsHelper.FillSingleLetterStats(inputStream1);
				}
				using (IReadOnlyStream inputStream2 = new ReadOnlyStream(args[1]))
				{
					doubleLetterStats = FillLetterStatsHelper.FillDoubleLetterStats(inputStream2);
				}

				FilterCharStats.RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
				FilterCharStats.RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

				PrintStatistic(singleLetterStats);
				PrintStatistic(doubleLetterStats);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				Console.ReadKey();
			}
		}

		/// <summary>
		/// Выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
		/// Каждая буква - с новой строки.
		/// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
		/// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
		/// </summary>
		/// <param name="letters">Коллекция со статистикой</param>
		private static void PrintStatistic(Dictionary<char, LetterStats> letters)
		{
			// TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
			IEnumerable<KeyValuePair<char, LetterStats>> orderCollection = letters.OrderBy(x => x.Key).Select(x => x);
			int uniqueCount = 0;
			int generalCount = 0;

			foreach (KeyValuePair<char, LetterStats> item in orderCollection)
			{
				Console.WriteLine(string.Format("{0} : {1}", item.Value.Letter, item.Value.Count));

				uniqueCount++;
				generalCount += item.Value.Count;
			}

			//По задаче не понял что имелось в виду под ИТОГО.
			Console.WriteLine("ИТОГО уникальных: " + uniqueCount);
			Console.WriteLine("ИТОГО сумма: " + generalCount);
			Console.WriteLine();
		}
	}
}