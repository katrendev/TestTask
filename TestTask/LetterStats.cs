namespace TestTask
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    //Изминил структуру на клаас, что бы при передачи в качестве аргумента метода IncStatistic(LetterStats letterStats) работало как ссылочный тип).
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
    }
}
