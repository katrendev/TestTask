using System;

namespace TestTask
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public class LetterStats : IEquatable<LetterStats>
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
        /// Сравнивает объект по значению
        /// </summary>
        /// <param name="other">Объект для сравнения</param>
        /// <returns></returns>
        public bool Equals(LetterStats other)
        {
            return this.Letter == other.Letter;
        }
    }
}
