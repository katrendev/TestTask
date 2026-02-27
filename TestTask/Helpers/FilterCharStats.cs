namespace TestTask.Helpers
{
	using System.Collections.Generic;
	using System.Linq;
	using TestTask.Enums;
	using TestTask.Helpers.Extensions;
	using TestTask.Structs;

	/// <summary>
	/// Класс для фильтрации статистики букв.
	/// </summary>
	internal static class FilterCharStats
	{
		/// <summary>
		/// Фильтруются все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
		/// (Тип букв для перебора определяется параметром charType)
		/// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
		/// </summary>
		/// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
		/// <param name="charType">Тип букв для анализа</param>
		public static void RemoveCharStatsByType(Dictionary<char, LetterStats> letters, CharType charType)
		{
			List<char> list = letters.Select(x => x.Key).ToList();

			foreach (char value in list)
			{
				switch (charType)
				{
					case CharType.Consonants:

						if (!value.IsVowel())
						{
							letters.Remove(value);
						}
						break;

					case CharType.Vowel:
						if (value.IsVowel())
						{
							letters.Remove(value);
						}
						break;
				}
			}
		}
	}
}