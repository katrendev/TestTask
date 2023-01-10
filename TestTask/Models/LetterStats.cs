namespace TestTask.Models
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public struct LetterStats
    {
        /// <summary>
        /// Буква/Пара букв для учёта статистики.
        /// </summary>
        public string Letter { get; set; }

        /// <summary>
        /// Кол-во вхождений буквы/пары.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Метод увеличивает счётчик вхождений.
        /// </summary>
        public void IncStatistic()
        {
            Count++;
        }

        public override string ToString()
        {
            return Letter + " " + Count;
        }
    }
}
