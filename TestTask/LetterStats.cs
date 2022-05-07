namespace TestTask
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public struct LetterStats
    {
        /// <summary>
        /// Буква/Пара букв для учёта статистики.
        /// </summary>
        public string Letter { get; }

        /// <summary>
        /// Кол-во вхождений буквы/пары.
        /// </summary>
        public int Count { get; }


        public LetterStats(string letter, int count)
        {
            Letter = letter;
            Count = count;
        }

        public LetterStats IncreaseCount()
        {
            return new LetterStats(Letter, Count + 1);
        }

    }
}
