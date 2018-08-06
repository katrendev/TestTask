using System.Collections.Generic;

namespace TestTask
{
    /// <summary>
    /// Тип букв
    /// </summary>
    public enum CharType
    {
        /// <summary>
        /// Гласные
        /// </summary>
        Vowel,

        /// <summary>
        /// Согласные
        /// </summary>
        Consonants
    }

    public static class CharTypeDetect
    {
        public const string Vowels = "aeiouаоиеёэыуюя";

        public static CharType DetectType(LetterStats LetterStats)
        {
            if (Vowels.Contains(LetterStats.Letter.ToLower()))
                return CharType.Vowel;
            else
                return CharType.Consonants;
        }

    }
}
