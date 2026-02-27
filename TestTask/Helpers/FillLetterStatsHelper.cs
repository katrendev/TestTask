namespace TestTask.Helpers
{
	using System.Collections.Generic;
	using TestTask.Streams.Interfaces;
	using TestTask.Structs;

	/// <summary>
	/// Хелпер для составления статистики букв.
	/// </summary>
	internal static class FillLetterStatsHelper
	{
		#region Public Methods

		/// <summary>
		/// Считывает из входящего потока все буквы, и возвращающает коллекцию статистик вхождения парных букв.
		/// </summary>
		/// <param name="stream">Стрим для считывания символов для последующего анализа</param>
		/// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
		/// <remarks>
		/// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
		/// Статистика - НЕ регистрозависимая!
		/// </remarks>
		public static Dictionary<char, LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
		{
			stream.ResetPositionToStart();

			var statsByLetter = new Dictionary<char, LetterStats>();
			char? prevLetter = null;

			while (!stream.IsEof)
			{
				char originalLetter = stream.ReadNextChar();

				// В статистику идут только буквы
				if (!char.IsLetter(originalLetter))
				{
					prevLetter = null;

					continue;
				}

				char letter = char.ToUpper(originalLetter); // НЕ регистрозависимо

				// Если текущая буква равна предыдущей — это парная буква (AA, CC, ...)
				if (prevLetter.HasValue && prevLetter.Value == letter)
				{
					// ВАЖНО: инкрементим статистику именно по букве
					if (statsByLetter.TryGetValue(letter, out LetterStats stats))
					{
						stats.Count++;
						statsByLetter[letter] = stats; // обязательно вернуть обратно
					}
					else
					{
						statsByLetter[letter] = new LetterStats { Count = 1, Letter = $"{letter}{letter}" };
					}
				}

				prevLetter = letter;
			}

			return statsByLetter;
		}

		/// <summary>
		/// Считывает из входящего потока все буквы, и возвращающает коллекцию статистик вхождения каждой буквы.
		/// </summary>
		/// <param name="stream">Стрим для считывания символов для последующего анализа</param>
		/// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
		/// <remarks>Статистика РЕГИСТРОЗАВИСИМАЯ!</remarks>
		public static Dictionary<char, LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
		{
			stream.ResetPositionToStart();
			Dictionary<char, LetterStats> statsByLetter = new Dictionary<char, LetterStats>();

			while (!stream.IsEof)
			{
				char letter = stream.ReadNextChar();

				if (char.IsLetter(letter))
				{
					if (statsByLetter.TryGetValue(letter, out LetterStats stats))
					{
						stats.Count++;
						statsByLetter[letter] = stats; // обязательно вернуть обратно
					}
					else
					{
						statsByLetter[letter] = new LetterStats { Count = 1, Letter = letter.ToString() };
					}
				}
			}

			return statsByLetter;
		}

		#endregion Public Methods
	}
}