using System.Collections.Generic;

namespace TestTask
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public class LetterStats
    {
        string _letter;
        private List<char> _vowel = new List<char>() { 'A', 'E', 'I', 'O', 'U', 'А', 'О', 'И', 'Е', 'Ё', 'Э', 'Ы', 'У', 'Ю', 'Я' };

        public LetterStats(string letter)
        {
            Letter = letter;
            Count = 0;
        }

        public LetterStats(char letter)
        {
            Letter = letter.ToString();
            Count = 0;
        }

        /// <summary>
        /// Буква/Пара букв для учёта статистики.
        /// </summary>
        public string Letter { get { return _letter; } set { _letter = value; IsVowel = _vowel.Contains(char.ToUpper(value[0])); } }

        /// <summary>
        /// Кол-во вхождений буквы/пары.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Тип буквы
        /// </summary>
        public CharType Type { get; set; }

        public bool IsVowel { get; private set; }
    }
}
