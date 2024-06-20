using System.Security.Cryptography.X509Certificates;

namespace TestTask
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public class LetterStats
    {
        /// <summary>
        /// Кол-во вхождений буквы/пары.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Буква/Пара букв для учёта статистики.
        /// </summary>
        public string Letter { get; set; }

        public LetterStats(string letter)
        {
            Letter = letter;
            Count = 1;
        }

        public override string ToString()
        {
            return $"{Letter} : {Count}";
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        public void IncStatistic()
        {
            Count++;
        }
    }
}
