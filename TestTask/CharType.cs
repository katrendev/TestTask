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

    /// <summary>
    /// Вспомогательный класс для классификации букв по типу.
    /// </summary>
    public static class CharTypeHelper
    {
        private const string RussianVowels = "аеёиоуыэюя";
        // TODO: добавить латинские гласные при необходимости: "aeiou"

        public static bool IsVowel(char c) =>
            RussianVowels.Contains(char.ToLower(c));
    }
}
