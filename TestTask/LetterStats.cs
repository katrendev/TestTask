namespace TestTask
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public struct LetterStats
    {
        string letter;
        int count;
        CharType charType;

        /// <summary>
        /// Буква/Пара букв для учёта статистики.
        /// </summary>
        public string Letter
        {
            get
            {
                return letter;
            }
            set
            {
                letter = value;
            }
        }

        /// <summary>
        /// Кол-во вхождений буквы/пары.
        /// </summary>
        public int Count
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
            }
        }

        /// <summary>
        /// Тип буквы/пары букв.
        /// </summary>
        public CharType CharType
        {
            get
            {
                return charType;
            }
            set
            {
                charType = value;
            }
        }
    }
}
