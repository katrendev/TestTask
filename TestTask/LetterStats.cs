namespace TestTask
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public struct LetterStats
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
        /// "Шаблон" для вывода статистики в виде <Буква/пара>: <Количество>
        /// </summary>
        public override string ToString() => $"{Letter}: {Count}";
        
        /// <summary>
        /// Тип звука (гласный/согласный)
        /// </summary>
        public readonly CharType charType;
        
        /// <summary>
        /// Конструктор с параметрами
        /// </summary>
        /// <param name="charType">Тип звука.</param>
        /// <param name="Letter">Буква/Пара букв.</param>
        /// <param name="Count">Кол-во вхождений буквы/пары.</param>
        public LetterStats(CharType charType, string Letter, int Count)
        {
            this.charType = charType;
            this.Letter = Letter;
            this.Count = Count;
        }
    }
}
