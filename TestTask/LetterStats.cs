using System;

namespace TestTask
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public class LetterStats: IEquatable<LetterStats> //используем класс чтобы явно обращаться к элементу коллекции
    {
        /// <summary>
        /// Буква/Пара букв для учёта статистики.
        /// </summary>
        public string Letter;

        /// <summary>
        /// Кол-во вхождений буквы/пары.
        /// </summary>
        public int Count;
        public LetterStats(string letter="") { Letter = letter;Count = 1; }

        public bool Equals(LetterStats other)
        {
            if (other == null) return false;
            return (this.Letter.Equals(other.Letter));
        }
    }
}
