using System.Collections.Generic;
using System;
using System.Linq;

namespace TestTask
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public class LetterStats
    {
        public LetterStats(string letter, CharType type, int count) 
        {
            Letter = letter;
            Type = type;
            Count = count;
        }
        /// <summary>
        /// Буква/Пара букв для учёта статистики.
        /// </summary>
        public string Letter { get; }
        /// <summary>
        /// Гласная/Согласная
        /// </summary>
        public CharType Type { get; }
        /// <summary>
        /// Кол-во вхождений буквы/пары.
        /// </summary>
        public int Count { get; set; }
    }
}
