namespace TestTask
{
    /// <summary>
    ///     Статистика вхождения буквы/пары букв
    /// </summary>
    public class LetterStats
    {
        public LetterStats(char letter)
        {
            Letter = letter;
            Count = 0;
        }
        
        /// <summary>
        ///     Увеличить счетчик вхождений на единицу
        /// </summary>
        public void IncCount()
        {
            Count++;
        }

        /// <summary>
        ///     Буква (или одна из набора букв) для учёта статистики.
        /// </summary>
        public char Letter { get; }

        /// <summary>
        ///     Кол-во вхождений буквы/пары.
        /// </summary>
        public int Count { get; private set; }
    }
}