namespace TestTask
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public class LetterStats
    {
        #region Public Properties

        /// <summary>
        /// Кол-во вхождений буквы/пары.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Буква/Пара букв для учёта статистики.
        /// </summary>
        public char Letter { get; }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Инициализирует свойства структуры <see cref="LetterStats"/>
        /// </summary>
        /// <param name="letter"></param>
        public LetterStats(char letter)
        {
            Letter = letter;
            Count = 0;
        }

        #endregion Public Constructors
    }
}
