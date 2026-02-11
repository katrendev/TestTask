using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTask.Helpers
{
    public static class HelperPrintStat
    {
        private const string __space = " ";
        private const string __colon = ":";
        private const string __total = "ИТОГО";
        private const string __empty = "[ПУСТО]";
        private static readonly string __rn = Environment.NewLine;

        /// <summary>
        /// Метод отчет по статистике.
        /// </summary>
        /// <param name="letters">Коллекция статистик.</param>
        /// <returns>Строковый отчет.</returns>
        public static string BuildReport(IEnumerable<LetterStats> letters)
        {
            if (!letters.Any())
                return __empty;
            var sortedLetters = SortedLetters(letters);
            var body = string.Join(__rn, BuildLines(sortedLetters));
            var total = BuildTotal(letters.Count());
            return $"{body}{__rn}{total}";
        }

        /// <summary>
        /// Метод сортирует коллекцию статистик по алфавиту.
        /// </summary>
        /// <param name="letters">Коллекция статистик.</param>
        /// <returns>Отсортированная коллекция статистик.</returns>
        private static IEnumerable<LetterStats> SortedLetters(IEnumerable<LetterStats> letters) =>
            letters
                .OrderBy(x => x.Letter);

        /// <summary>
        /// Метод строит коллекцию строк из коллекции статистик.
        /// </summary>
        /// <param name="letters">Входная коллекция статистик.</param>
        /// <returns>Коллекция строковых статистик.</returns>
        private static IEnumerable<string> BuildLines(IEnumerable<LetterStats> letters) =>
            letters
                .Select(x => BuildLine(x));

        /// <summary>
        /// Метод строит строку из статистики.
        /// </summary>
        /// <param name="stat">Статистика.</param>
        /// <returns>Строковая статистика.</returns>
        private static string BuildLine(LetterStats stat) => $"{stat.Letter}{BuildBreak()}{stat.Count}";

        /// <summary>
        /// Метод строит строку ИТОГО.
        /// </summary>
        /// <param name="count">Количество для ИТОГО.</param>
        /// <returns>Строка ИТОГО со значением.</returns>
        private static string BuildTotal(int count) => $"{__total}{BuildBreak()}{count}";

        /// <summary>
        /// Метод строит строку для отделения значений.
        /// </summary>
        /// <returns>строка с разделителями.</returns>
        private static string BuildBreak() => $"{__space}{__colon}{__space}"; 
    }
}
