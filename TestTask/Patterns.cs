namespace TestTask
{

    /// <summary>
    /// Шаблоны поиска для регулярных выражений
    /// </summary>
    public static class Patterns
    {

        /// <summary>
        /// Кириллица
        /// </summary>
        public const string Cyrillic = @"\p{IsCyrillic}";

        /// <summary>
        /// Кириллический мягкий знак
        /// </summary>
        public const string SoftSign = @"[ь]";

        /// <summary>
        /// Кириллический твёрдый знак
        /// </summary>
        public const string SolidSign = @"[ъ]";

        /// <summary>
        /// Кириллические согласные
        /// </summary>
        public const string Consonants = @"[бвгджзйклмнпрстфхцчшщ]";

        /// <summary>
        /// Кириллические гласные
        /// </summary>
        public const string Vowels = @"[аеёиоуыэюя]";

        /// <summary>
        /// Дубликаты
        /// </summary>
        public const string Duplicates = @"(\w)\1";

    }

}
