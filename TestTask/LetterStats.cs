using System.Collections.Generic;

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
        public char Letter { get; private set; }

        /// <summary>
        /// Кол-во вхождений буквы/пары.
        /// </summary>
        public int Count { get; private set; }

        public LetterStats(char letter) : this()
        {
            Letter = letter;
        }

        public LetterStats(char letter, int count) : this()
        {
            Letter = letter;
            Count = count;
        }

        public void IncrementCount()
        {
            Count++;
        }
    }
}
