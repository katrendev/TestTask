namespace TestTask.Helpers
{
    /// <summary>
    /// Представляет набор констант для анализа букв на предмет гласности, согласности.
    /// </summary>
    internal static class CharTypeHelper
    {
        /// <summary>
        /// Набор гласных букв для русского языка.
        /// </summary>
        internal const string RUS_EN_VOWELS = "АУОЫИЭЯЮЁЕAEIOUY";

        /// <summary>
        /// Набор согласных букв для русского языка.
        /// </summary>
        internal const string RUS_EN_CONSONANTS = "БВГДЖЗЙКЛМНПРСТФХЦЧШЩBCDFGHJKLMNPQRSTVWXZ";
    }
}
