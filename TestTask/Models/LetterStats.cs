using System;

namespace TestTask.Models
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public struct LetterStats<T>
    {
        /// <summary>
        /// Буква/Пара букв для учёта статистики.
        /// </summary>
        public T Letter { get; }

        /// <summary>
        /// Кол-во вхождений буквы/пары.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Инициализирует экземпляр <see cref="LetterStats{T}"/>.
        /// </summary>
        /// <param name="letter">Статистика какого значения.</param>
        /// <param name="count">Кол-во вхождений</param>
        public LetterStats(T letter, int count)
        {
            Letter = letter;
            Count = count;
        }
    }
}