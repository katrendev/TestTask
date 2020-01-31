using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestTask.Helpers;
using TestTask.Models;

namespace TestTask.Services
{
	public static class LettersAnalyzer
	{
		/// <summary>
		/// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
		/// Статистика РЕГИСТРОЗАВИСИМАЯ!
		/// </summary>
		/// <param name="stream">Стрим для считывания символов для последующего анализа</param>
		/// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
		public static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
		{
			var statsDictionary = new Dictionary<char, LetterStats>();
			stream.ResetPositionToStart();
			while (!stream.IsEof)
			{
				var c = stream.ReadNextChar();
				if (!char.IsLetter(c))
					continue;

				if (statsDictionary.ContainsKey(c))
					statsDictionary[c].IncStatistic();
				else statsDictionary.Add(c, new LetterStats(c.ToString()));
			}
			stream.Dispose();
			return statsDictionary.Values.ToList();
		}

		/// <summary>
		/// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
		/// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
		/// Статистика - НЕ регистрозависимая!
		/// </summary>
		/// <param name="stream">Стрим для считывания символов для последующего анализа</param>
		/// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
		public static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
		{
			char? prev = null;
			var statsDictionary = new Dictionary<string, LetterStats>();
			stream.ResetPositionToStart();
			while (!stream.IsEof)
			{
				var c = char.ToUpper(stream.ReadNextChar());
				if (!char.IsLetter(c))
				{
					prev = null;
					continue;
				}

				if (prev.HasValue && prev == c)
				{
					prev = null;
					var pair = $"{c}{c}";
					if (statsDictionary.ContainsKey(pair))
						statsDictionary[pair].IncStatistic();
					else statsDictionary.Add(pair, new LetterStats(pair));

					continue;
				}
				prev = c;
			}
			stream.Dispose();
			return statsDictionary.Values.ToList();
		}

		/// <summary>
		/// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
		/// (Тип букв для перебора определяется параметром charType)
		/// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
		/// </summary>
		/// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
		/// <param name="charType">Тип букв для анализа</param>
		public static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
		{
			switch (charType)
			{
				case CharType.Consonants:
					letters.RemoveAll(x => !CharHelper.Vowels.ContainsKey(char.ToUpper(x.Letter.First())));
					break;
				case CharType.Vowel:
					letters.RemoveAll(x => CharHelper.Vowels.ContainsKey(char.ToUpper(x.Letter.First())));
					break;
			}
		}

		/// <summary>
		/// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
		/// Каждая буква - с новой строки.
		/// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
		/// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
		/// </summary>
		/// <param name="letters">Коллекция со статистикой</param>
		public static void PrintStatistic(IEnumerable<LetterStats> letters)
		{
			if (letters == null)
				return;

			var sb = new StringBuilder();
			foreach (var letter in letters.OrderBy(x => x.Letter))
				sb.Append($"{letter.Letter} : {letter.Count} \n");

			Console.Write(sb.ToString());

			Console.WriteLine($"Итого {letters.Select(x => x.Count).Sum()} вхождений для {letters.Count()} букв");
		}
	}
}