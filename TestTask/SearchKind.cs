using System.ComponentModel;

namespace TestTask
{
    /// <summary>
    /// Вид поиска
    /// </summary>
    public enum SearchKind
    {
        /// <summary>
        /// одиночные буквы
        /// </summary>
        SingleLetters,
        /// <summary>
        /// парные буквы
        /// </summary>
        DoubleLetters
    }

    /// <summary>
    /// Расширение для типа <see cref="SearchKind"/>
    /// </summary>
    public static class SearchKindExtension
    {
        /// <summary>
        /// Возвращает человекопонятное описание заданного элемента перечисления видов поиска
        /// </summary>
        /// <param name="searchKind">вид поиска</param>
        /// <returns>человекопонятное описание</returns>
        public static string ToFriendlyString(this SearchKind searchKind)
        {
            return searchKind == SearchKind.DoubleLetters ? "парные буквы" : "одиночные буквы";
        }
    }
}