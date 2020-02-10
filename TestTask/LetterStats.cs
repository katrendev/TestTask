using System;

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

        public CharType CharType;

        public LetterStats(string letter)
        {
            Letter = letter;
            CharType = letter[0].GetCharType();
            IncStatistic();
        }
        public void IncStatistic()
        {
            Count++;
        }
    }
    public static class CharacterExtentions
    {
        public static CharType GetCharType(this char c)
        {
            return "aeioujуеыаоэяию".IndexOf(c.ToString(), StringComparison.CurrentCultureIgnoreCase) >= 0 ? CharType.Vowel : CharType.Consonants;
        }
    }


}
