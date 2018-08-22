namespace TestTask
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public class LetterStats
    {
        public LetterStats(string Letter,int InitCount = 0)
        {
            this.Letter = Letter;
            this.Count = InitCount;
        }
        
        /// <param name="doubleLetter">True - для пары букв</param>
        /// <param name="InitCount">Начальное значение счетчика</param>
        public LetterStats(char Letter, bool doubleLetter = false, int InitCount = 0) 
            : this(
                  Letter.ToString()+ (doubleLetter?Letter.ToString():""), 
                  InitCount) { }

        /// <summary>
        /// Буква/Пара букв для учёта статистики.
        /// </summary>
        public readonly string Letter;

        /// <summary>
        /// Кол-во вхождений буквы/пары.
        /// </summary>
        public int Count;
    }
}
