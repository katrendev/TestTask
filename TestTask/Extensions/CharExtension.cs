using System.Linq;

namespace TestTask.Extensions
{
    /// <summary>
    /// Метод расширение для char
    /// </summary>
    public static class CharExtension
    {
        private const string vowels = "аеёиоуыэюяaeiou";

        /// <summary>
        /// Проверяем является ли весь поданный массив символов гласными
        /// Только EN,RU
        /// </summary>
        /// <param name="c">Проверяемые символы</param>
        /// <returns>Является ли символы гласными</returns>
        public static bool IsVowel(this char[] chars)
            => chars.All(c => c.IsVowel());

        /// <summary>
        /// Проверяем является ли поданный символ гласным
        /// Только EN,RU
        /// </summary>
        /// <param name="c">Проверяемый символ</param>
        /// <returns>Является ли символ гласным</returns>
        public static bool IsVowel(this char c)
        {
            c = char.ToLower(c);
            return vowels.Contains(c);
        }

        /// <summary>
        /// Проверяем является ли весь поданный массив символов согласными
        /// Только EN,RU
        /// </summary>
        /// <param name="c">Проверяемые символы</param>
        /// <returns>Является ли символы согласными</returns>
        public static bool IsConsonant(this char[] chars)
            => !IsVowel(chars);

        /// <summary>
        /// Проверяем является ли поданный символ согласным
        /// Только EN,RU
        /// </summary>
        /// <param name="c">Проверяемый символ</param>
        /// <returns>Является ли символ согласным</returns>
        public static bool IsConsonant(this char c)
            => !IsVowel(c);

        /// <summary>
        /// Возвращает значение, указывающее, равен ли этот экземпляр указанному
        /// </summary>
        /// <param name="obj">Символ для сравнения с текущим</param>
        /// <param name="ignoreCase">Определяет будет ли игнорироваться регистр проверяемых символов</param>
        /// <returns>true, если значение параметра value совпадает с этой строкой; в противном случае - false</returns>
        public static bool Equals(this char c, char obj, bool ignoreCase)
            => ignoreCase ? char.ToLower(c) == char.ToLower(obj) : c == obj;
    }
}
