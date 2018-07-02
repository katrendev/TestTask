namespace TestTask
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public class LetterStats
    {
        /// <summary>
        /// Буква/Пара букв для учёта статистики.
        /// </summary>
        public string Letter;

        /// <summary>
        /// Кол-во вхождений буквы/пары.
        /// </summary>
        public int Count;

        /// <summary>
        /// Переобразует экземпляр объекта в текстовую строку
        /// </summary>
        public override string ToString()
        {
            return "{" + Letter + "} : {" + Count + "}";
        }
    }
}
