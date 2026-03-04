using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
	/// <summary>
	/// Контейнер для подсчёта количества вхождений текста с помощью словаря
	/// </summary>
	public sealed class DictionaryLetterStorage : ILetterAnalysisStorage
	{
		private Dictionary<string, LetterStats> _data = new Dictionary<string, LetterStats>();

		/// <summary>
		/// Обрабатывает статистику по введённому тексту 
		/// </summary>
		///<param name="text"></param>
		public void Add(string text)
		{
			if (_data.TryGetValue(text, out LetterStats stats))
			{
				IncStatistic(text, stats);
			} 
			else
			{
                stats = new LetterStats
				{
					Letter = text.ToString(),
					Count = 1
				};
				_data.Add(text, stats);
			}
		}

		/// <summary>
		/// Метод увеличивает счётчик вхождений по переданной структуре.
		/// </summary>
		/// <param name="letterStats"></param>
		private void IncStatistic(string key, LetterStats value)
		{
			value.Count++;
            _data[key] = value;
		}

		/// <summary>
		/// Возвращает список с данными вхождений текста
		/// </summary>
		/// <returns>Список с данными вхождений текста</returns>
		public IList<LetterStats> GetStatistics() => _data.Values.ToList();
	}
}