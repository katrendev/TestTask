using System.Linq;

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

    public static class CharExt
    {
        /// <summary>
        /// Проверка принадлежности символа к гласным
        /// </summary>
        /// <param name="c">Проверяемый символ</param>
        /// <returns>Да - гласный, нет - согласный</returns>
        public static bool IsVowel(this char c)
        {
            var lowerC = char.ToLower(c);
            return "aeiou".Contains(lowerC) || "аеёиоуыэюя".Contains(lowerC);
        }
    }
}
