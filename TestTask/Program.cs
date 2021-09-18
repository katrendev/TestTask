using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace TestTask
{
	public class Program
	{
		/// <summary>
		/// Константа определяет какое количество подряд идущих одинаковых букв учитывается в статистике
		/// </summary>
		private const int N = 2;
		/// <summary>
		/// Программа принимает на входе 2 пути до файлов.
		/// Анализирует в первом файле кол-во вхождений каждой буквы (регистрозависимо). Например А, б, Б, Г и т.д.
		/// Анализирует во втором файле кол-во вхождений парных букв (не регистрозависимо). Например АА, Оо, еЕ, тт и т.д.
		/// По окончанию работы - выводит данную статистику на экран.
		/// </summary>
		/// <param name="args">Первый параметр - путь до первого файла.
		/// Второй параметр - путь до второго файла.</param>
		static void Main(string[] args)
		{

			IReadOnlyStream inputStream1 = GetInputStream(args[0]);
			IReadOnlyStream inputStream2 = GetInputStream(args[1]);

			IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
			PrintStatistic(singleLetterStats);

			IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);
			PrintStatistic(doubleLetterStats);

			RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
			RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

			PrintStatistic(singleLetterStats);
			PrintStatistic(doubleLetterStats);

			// TODO : Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
			Console.ReadKey();
		}

		/// <summary>
		/// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
		/// </summary>
		/// <param name="fileFullPath">Полный путь до файла для чтения</param>
		/// <returns>Поток для последующего чтения.</returns>
		private static IReadOnlyStream GetInputStream(string fileFullPath)
		{
			return new ReadOnlyStream(fileFullPath);
		}

		/// <summary>
		/// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
		/// Статистика РЕГИСТРОЗАВИСИМАЯ!
		/// </summary>
		/// <param name="stream">Стрим для считывания символов для последующего анализа</param>
		/// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
		private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
		{
			stream.ResetPositionToStart();
			var letterDict = new Dictionary<char, LetterStats>();

			while (!stream.IsEof)
			{
				char c = stream.ReadNextChar();

				if (!char.IsLetter(c)) continue;

				// TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
				if (!letterDict.ContainsKey(c))
					letterDict.Add(c, new LetterStats(c.ToString()));
				else
					IncStatistic(letterDict[c]);
			}

			return letterDict.Values.ToList();
		}

		/// <summary>
		/// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
		/// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
		/// Статистика - НЕ регистрозависимая!
		/// </summary>
		/// <param name="stream">Стрим для считывания символов для последующего анализа</param>
		/// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
		private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
		{
			stream.ResetPositionToStart();
			
			var letters = new Dictionary<string, LetterStats>();
			var folder = new char[N];

			var current = stream.ReadNextChar();
			while (!char.IsLetter(current) && !stream.IsEof)
				current = stream.ReadNextChar();

			int position = 0;
			folder[position] = current;
			
			while (!stream.IsEof)
			{
				char next = stream.ReadNextChar();

				if (!char.IsLetter(next))
				{
					position = 0;
					current = new char();
					folder.Initialize();
					continue;
				}

				position = char.ToUpperInvariant(current) == char.ToUpperInvariant(next) && position < N - 1 ? position + 1 : 0;
				folder[position] = next;
				current = next;

				// TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.
				if (position == N - 1)
				{
					var pair = new string(folder).ToUpperInvariant();
					
					if (!letters.ContainsKey(pair))
						letters.Add(pair, new LetterStats(pair));
					else
					  IncStatistic(letters[pair]);
				}
			}

			return letters.Values.ToList();
		}

		/// <summary>
		/// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
		/// (Тип букв для перебора определяется параметром charType)
		/// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
		/// </summary>
		/// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
		/// <param name="charType">Тип букв для анализа</param>
		private static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
		{
			string pattern = "";
			// TODO : Удалить статистику по запрошенному типу букв.
			switch (charType)
			{
				case CharType.Consonants:
					pattern = @"^[йцкнгшщзхфвпрлджбтмсч]*$";
					break;
				case CharType.Vowel:
					pattern = @"^[аеёиоуыэюя]*$";
					break;
			}

			var rgx = new Regex(pattern, RegexOptions.IgnoreCase);

			for (int i = 0; i < letters.Count; i++)
			{
				if (rgx.IsMatch(letters[i].Letter))
				{
					letters.Remove(letters[i]);
					i--;
				}
			}

		}

		/// <summary>
		/// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
		/// Каждая буква - с новой строки.
		/// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
		/// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
		/// </summary>
		/// <param name="letters">Коллекция со статистикой</param>
		private static void PrintStatistic(IEnumerable<LetterStats> letters)
		{
			if (letters == null || !letters.Any()) return;

			// TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
			foreach (var i in letters.OrderBy(x => x.Letter))
			{
				Console.WriteLine($"{i.Letter} : {i.Count}");
			}

			var sum = letters.Select(x => x.Count).Sum();

			Console.WriteLine($"ИТОГО  - {sum}");
		}

		/// <summary>
		/// Метод увеличивает счётчик вхождений по переданной структуре.
		/// </summary>
		/// <param name="letterStats"></param>
		private static void IncStatistic(LetterStats letterStats)
		{
			letterStats.Count++;
		}


	}
}
