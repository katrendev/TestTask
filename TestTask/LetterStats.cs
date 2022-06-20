namespace TestTask
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public struct LetterStats
    {
        #region Public Constructors

        /// <summary>
        /// Инициализирует экземпляр <see cref="LetterStats"/>.
        /// </summary>
        /// <param name="letter">Буква/Пара букв для учёта статистики</param>
        public LetterStats(char letter)
        {
            Letter = letter;
            Count = 0;
        }

        /// <summary>
        /// Инициализирует экземпляр <see cref="LetterStats"/>.
        /// </summary>
        /// <param name="letter">Буква/Пара букв для учёта статистики</param>
        /// <param name="count">Кол-во вхождений буквы/пары.</param>
        public LetterStats(char letter, int count)
        {
            Letter = letter;
            Count = count;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Кол-во вхождений буквы/пары.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Буква/Пара букв для учёта статистики.
        /// </summary>
        public char Letter { get; private set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Увеличивает кол-во вхождений на 1.
        /// </summary>
        public void IncreaseCount()
        {
            Count++;
        }

        #endregion Public Methods
    }
}