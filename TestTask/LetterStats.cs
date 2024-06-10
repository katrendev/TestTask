namespace TestTask
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public class LetterStats
    {
        public LetterStats(char c)
        {
            Letter = c;
            Count = 1;
        }
        /// <summary>
        /// Буква/Пара букв для учёта статистики.
        /// </summary>
        public char Letter;

        /// <summary>
        /// Кол-во вхождений буквы/пары.
        /// </summary>
        public int Count;
    }
}
