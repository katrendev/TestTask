using System.Collections.Generic;

namespace TestTask
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public class LetterStats
    {
        /// <summary>
        /// Буква/Пара букв для учёта статистики.
        /// </summary>
        public string Letter;

        /// <summary>
        /// Кол-во вхождений буквы/пары.
        /// </summary>
        public int Count;
    }

    public class CaseSensetiveComparer : IEqualityComparer<LetterStats>
    {
        public bool Equals(LetterStats x, LetterStats y)
        {
            return x.Letter == y.Letter;
        }

        public int GetHashCode(LetterStats ls)
        {
            return ls.GetHashCode();
        }
    }
}
