using System;
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

    public static class CharTypeExtensions
    {
        public static CharType GetCharType(this LetterStats letterStats)
        {
            var rusVowels = new HashSet<string>()
            {
                "ё", "у", "е", "ы", "а", "о", "э", "я", "и", "ю",
                "Ё", "У", "Е", "Ы", "А", "О", "Э", "Я", "И", "Ю",
            };
            var enVowels = new HashSet<string>()
            {
                "e", "y", "u", "i", "o", "a",
                "E", "Y", "U", "I", "O", "A",
            };

            var vowel = rusVowels.Contains(letterStats.Letter) || enVowels.Contains(letterStats.Letter);
            
            return vowel ? CharType.Vowel : CharType.Consonants;
        }
    }
}
