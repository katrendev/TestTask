using System.Collections.Generic;
using System.Linq;
using TestTask.Io;

namespace TestTask.Stats
{
    /// <summary>
    /// Базовый класс сборщика статистики вхождения символов в поток
    /// </summary>
    public class LetterStatsCollector
    {
        public static readonly LetterStatsCollector Instance = new LetterStatsCollector();

        /// <summary>
        /// Собирает статистику вхождения разрешённых символов в поток
        /// </summary>
        /// <param name="stream">Входной поток символов</param>
        /// <returns>Список элементов статистики</returns>
        public IList<LetterStatItem> Collect(IReadOnlyStream stream)
        {
            IDictionary<char, LetterStatItem> resultDict = new Dictionary<char, LetterStatItem>();
            // Отсюда я сознательно убрал вызов stream.ResetPositionToStart(),
            // т.к. это зона ответственности вызывающего, а не этого метода
            while (!stream.IsEof)
            {
                var ch = stream.ReadNextChar();
                if (!IsCharAllowed(ch))
                    continue;

                var chKey = NormalizeChar(ch);

                if (resultDict.TryGetValue(chKey, out var item))
                    IncStatistic(item, ch);
                else
                    resultDict.Add(chKey, new LetterStatItem {Letter = chKey, Count = 1});
            }

            return resultDict.Values.ToList();
        }

        /// <summary>
        /// Определяет, должен ли указанный символ попасть в статистику
        /// </summary>
        protected virtual bool IsCharAllowed(char ch) => char.IsLetter(ch);

        /// <summary>
        /// Нормализует указанный символ (нужно для объединения парных букв в группы)
        /// </summary>
        protected virtual char NormalizeChar(char ch) => ch;

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStatItem">Элемент статистики</param>
        /// <param name="ch">Свежедобавленный символ</param>
        private static void IncStatistic(LetterStatItem letterStatItem, char ch)
        {
            letterStatItem.Count++;
        }
    }
}