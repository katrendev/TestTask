using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
	public class Program
	{
		#region Private Methods

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

			LetterStats currentLetterPairStats;
			List<LetterStats> statsList = new List<LetterStats>();
			string currentLetter, currentLetterPair, previousLetter;

			previousLetter = stream.ReadNextChar().ToUpper();

			while (!stream.IsEof)
			{
				currentLetter = stream.ReadNextChar().ToUpper();

				if (currentLetter == previousLetter)
				{
					currentLetterPair = previousLetter + currentLetter;
					currentLetterPairStats = statsList.Find(x => x.Letter == currentLetterPair);

					if (currentLetterPairStats != null)
					{
						IncStatistic(currentLetterPairStats);
					}
					else
					{
						currentLetterPairStats = new LetterStats(currentLetterPair, 1);
						statsList.Add(currentLetterPairStats);
					}
				}

				previousLetter = currentLetter;
			}

			return statsList;
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

			List<LetterStats> statsList = new List<LetterStats>();

			while (!stream.IsEof)
			{
				string currentLetter = stream.ReadNextChar();
				LetterStats currentLetterStats = statsList.Find(x => x.Letter == currentLetter);

				if (currentLetterStats != null)
				{
					IncStatistic(currentLetterStats);
				}
				else
				{
					currentLetterStats = new LetterStats(currentLetter, 1);
					statsList.Add(currentLetterStats);
				}
			}

			return statsList;
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
		/// Метод увеличивает счётчик вхождений по переданной структуре.
		/// </summary>
		/// <param name="letterStats"></param>
		private static void IncStatistic(LetterStats letterStats)
		{
			letterStats.Count++;
		}

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
			IReadOnlyStream inputStream1 = GetInputStream(args[0]);
			IReadOnlyStream inputStream2 = GetInputStream(args[1]);

			IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
			IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

			RemoveCharStatsByType(ref singleLetterStats, CharType.Vowel);
			RemoveCharStatsByType(ref doubleLetterStats, CharType.Consonants);

			PrintStatistic(singleLetterStats);
			PrintStatistic(doubleLetterStats);

			inputStream1.Dispose();
			inputStream2.Dispose();

			Console.ReadLine();
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
			IOrderedEnumerable<LetterStats> sortedLetters = letters.ToArray().OrderBy(x => x.Letter);

			foreach (LetterStats letterStats in sortedLetters)
			{
				Console.WriteLine($"{letterStats.Letter} -> {letterStats.Count}");
			}

			Console.WriteLine();
		}

		/// <summary>
		/// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
		/// (Тип букв для перебора определяется параметром charType)
		/// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
		/// </summary>
		/// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
		/// <param name="charType">Тип букв для анализа</param>
		private static void RemoveCharStatsByType(ref IList<LetterStats> letters, CharType charType)
		{
			string vowels = "AEIOUYАЯУЮОЕЁЭИЫ";

			switch (charType)
			{
				case CharType.Consonants:
					letters = letters.Where(letter => vowels.Contains(letter.Letter.ToUpper().First())).ToList();
					break;

				case CharType.Vowel:
					letters = letters.Where(letter => !vowels.Contains(letter.Letter.ToUpper().First())).ToList();
					break;
			}
		}

		#endregion Private Methods
	}
}