using System;

namespace TestTask
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public class LetterStats
    {
        private string _letter;
        /// <summary>
        /// Буква/Пара букв для учёта статистики.
        /// </summary>
        public string Letter 
        {
            get => _letter;
            private set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(nameof(Letter));
                }
                _letter = value;
            }
        }

        private int _count;
        /// <summary>
        /// Кол-во вхождений буквы/пары.
        /// </summary>
        public int Count
        {
            get => _count;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _count = value;
            }
        }

        public LetterStats(string letter, int count = 0)
        {
            Letter = letter;
            Count = count;
        }

        public override string ToString()
        {
            return $"{Letter} : {Count}";
        }
    }
}
