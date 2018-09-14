using System.ComponentModel;

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
        [Description("гласные")] //Атрибут для более удобного вывода в консоль
        Vowel,

        /// <summary>
        /// Согласные
        /// </summary>
        [Description("согласные")] //Атрибут для более удобного вывода в консоль
        Consonants
    }
}
