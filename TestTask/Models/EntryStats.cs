namespace TestTask.Models
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв.
    /// </summary>
    public struct EntryStats
    {
        #region Public Constructors

        /// <summary>
        /// Инициализирует экземпляр <see cref="EntryStats{T}"/>.
        /// </summary>
        /// <param name="entry">Статистика записей.</param>
        /// <param name="count">Кол-во вхождений</param>
        public EntryStats(string entry, int count)
        {
            Entry = entry;
            Count = count;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Кол-во вхождений строки.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Строка для учёта статистики.
        /// </summary>
        public string Entry { get; }

        #endregion Public Properties
    }
}