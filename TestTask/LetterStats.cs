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
        public string Letter;

        /// <summary>
        /// Кол-во вхождений буквы/пары.
        /// </summary>
        public int Count;

        /// <summary>
        /// Гласная или согласная буква/пара.
        /// </summary>
        public CharType Type;

        private static readonly HashSet<char> _vovels = new HashSet<char>("AaEeIiOoUuYyАаОоУуЭэЫыЯяЁёЕеЮюИи");

        public LetterStats(string letter, int count)
        {
            Letter = letter;
            Count = count;

            Type = (_vovels.Contains(letter[0])) ? CharType.Vowel : CharType.Consonants;
        }
    }
}
