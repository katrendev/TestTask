using System;

namespace TestTask
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public struct LetterStats : IComparable
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
        /// Переопределение для вывода на экран данных структуры.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => string.Format($"{Letter} - {Count}\n");

        /// <summary>
        /// Сравнивает позиции текущей и заданной структур. (для сортировки по алфавиту).
        /// </summary>
        /// <param name="obj">Структура для сравнения с текущей.</param>
        /// <returns>Целое число, характеризующее отношение позиции.</returns>
        public int CompareTo(object obj)
        {
            LetterStats? tmp = (LetterStats)obj;
            return Letter.CompareTo(tmp?.Letter);
        }

        /// <summary>
        /// Проверка на равенство текущей и заданной структур. 
        /// </summary>
        /// <param name="obj">Структура для сравнения с текущей.</param>
        /// <returns>True если структуры равны по букве/паре букв, false в остальных случаях.</returns>
        public override bool Equals(object obj)
        {
            LetterStats? tmp = (LetterStats)obj;
            if (tmp == null) return false;
            else return Letter == tmp?.Letter;
        }

        /// <summary>
        /// Хэш-функция для для буквы/пары структуры.
        /// </summary>
        /// <returns>Хэш-код по полю Letter.</returns>
        public override int GetHashCode() => Letter.GetHashCode();
    }
}
