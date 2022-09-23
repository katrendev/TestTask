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

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public bool Equals(LetterStats other)
        {
            return Letter == other.Letter;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Letter != null ? Letter.GetHashCode() : 0) * 397) ^ Count;
            }
        }
    }
}
