namespace TestTask
{
	/// <summary>
	/// Статистика вхождения буквы/пары букв
	/// </summary>
	public class LetterStats
	{
		#region Public Fields

		/// <summary>
		/// Кол-во вхождений буквы/пары.
		/// </summary>
		public int Count;

		/// <summary>
		/// Буква/Пара букв для учёта статистики.
		/// </summary>
		public string Letter;

		#endregion Public Fields

		#region Public Constructors

		/// <summary>
		/// Инициализирует объект класса <see cref="LetterStats"/>
		/// </summary>
		public LetterStats(string Letter, int Count)
		{
			this.Letter = Letter;
			this.Count = Count;
		}

		#endregion Public Constructors
	}
}