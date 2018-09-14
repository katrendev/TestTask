namespace TestTask
{
    // Лучше использовать класс(ref type), а не структуру, т.к. в противном случае придется заменять всю структуру в списке целиком.
    
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public class LetterStats
    {
        public LetterStats(string letter)
        {
            Letter = letter;
            Count = 1; // По умолчанию 1, т.к. статистику ведем только для найденных букв/пар
        }

        /// <summary>
        /// Буква/Пара букв для учёта статистики.
        /// </summary>
        public string Letter { get; private set; }

        /// <summary>
        /// Кол-во вхождений буквы/пары.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Увеличивает счетчик на единицу
        /// </summary>
        public void IncCount()
        {
            Count++;
        }
    }
}
