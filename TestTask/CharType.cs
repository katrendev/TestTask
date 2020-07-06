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
    /// Cодержит определение всех гласных 
    /// </summary>
    public static class CharVowel 
    {        
        /// <summary>
        /// Гласные в двух регистрах, чтобы снизить время на преобразование 
        /// </summary>
        public const string VOWEL = "аеёиоуыэюяАЕЁИОУЫЭЮЯaeyuiojAEYUIOJ"; 
    }
}
