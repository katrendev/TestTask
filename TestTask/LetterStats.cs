using System.Collections.Generic;
using System.Linq;

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
        /// Набор глaсных букв для фильтрации статистики по гласным/согласным.
        /// Поддерживаем только латинские буквы (без диакритик) и кириллицу.
        /// </summary>
        private static readonly HashSet<char> _vowels = new HashSet<char>
        {
            'a', 'e', 'i', 'o', 'u', 'y',
            'а', 'е', 'ё', 'и', 'о', 'у', 'ы', 'э', 'ю', 'я'
        };

        /// <summary>
        /// Определяет относится ли данный элемент статистики к гласной или согласной букве (или паре одинаковыз букв)
        /// </summary>
        public CharType CharType
        {
            get
            {
                char ch = char.ToLower(Letter.First());
                return _vowels.Contains(ch) ? CharType.Vowel : CharType.Consonants;
            }
        }
    }
}
