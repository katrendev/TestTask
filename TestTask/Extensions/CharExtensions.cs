using System.Linq;

namespace TestTask.Extension
{
    public static class CharExtensions
    {
        /// <summary>
        /// Расширение для типа данных char
        /// </summary>
        /// <returns>Результат, является ли символ гласной буквой</returns>
        /// <param name="c">Символ, который необходимо проверить</param>
        public static bool IsVowel(this char c)
        {
            return "AEIOUАЕЁИОУЫЭЮЯ".Contains(char.ToUpper(c));
        }

        /// <summary>
        /// Расширение для типа данных char
        /// </summary>
        /// <returns>Результат, является ли символ согласной буквой</returns>
        /// <param name="c">Символ, который необходимо проверить</param>
        public static bool IsConsonant(this char c)
        {
            return char.IsLetter(c) && !IsVowel(c);
        }
    }
}
