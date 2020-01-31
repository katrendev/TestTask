namespace TestTask
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public class LetterStats
    {
	    public LetterStats(string letter)
	    {
		    Letter = letter;
		    Count = 1;
	    }

		/// <summary>
        /// Буква/Пара букв для учёта статистики.
        /// </summary>
        public string Letter;

        /// <summary>
        /// Кол-во вхождений буквы/пары.
        /// </summary>
        public int Count;

	    /// <summary>
	    /// Метод увеличивает счётчик вхождений
	    /// </summary>
	    public void IncStatistic()
	    {
		    Count++;
	    }
	}
}
