using System;

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
        public string Letter { get; private set; }

        /// <summary>
        /// Кол-во вхождений буквы/пары.
        /// </summary>
        public int Count { get; private set; }

        public CharType charsType { get; private set; }

        public static LetterStats operator ++(LetterStats stats)
        {
            stats.Count++;
            return stats;
        }

        public LetterStats (string letter)
        {
            if (letter.Length == 0 || letter.Length > 2 )
            {

                throw new Exception($"Letter must be legnth 1 or 2, but now {letter.Length}");
            }
            if (letter.Length == 2 && !CharExtensions.CompareIgnoreCases(letter[0],letter[1]))
            {
                throw new Exception($"Letter must be equal");
            }
            Letter = letter;
            Count = 0;
            var basicChar = letter[0];
            charsType = basicChar.GetCharType();
        }

        public LetterStats (char letter)
        {
            Letter = letter.ToString();
            Count = 0;
            charsType = letter.GetCharType();
        }
    }
}
