using System.Collections.Generic;
using System.Linq;
using TestTask.Enums;
using TestTask.EventsArgs;
using TestTask.Helpers;
using TestTask.Models;
using TestTask.Services;

namespace TestTask
{
    public class Program
    {
        #region Private Methods

        /// <summary>
        /// Сервис статистики.
        /// </summary>
        private static StatisticService _statisticService;

        /// <summary>
        /// Инициализирует компоненты.
        /// </summary>
        private static void InitializeComponents()
        {
            _statisticService = new StatisticService();
        }

        /// <summary>
        /// Программа принимает на входе 2 пути до файлов.
        /// Анализирует в первом файле кол-во вхождений каждой буквы (регистрозависимо). Например А, б, Б, Г и т.д.
        /// Анализирует во втором файле кол-во вхождений парных букв (не регистрозависимо). Например АА, Оо, еЕ, тт и т.д.
        /// По окончанию работы - выводит данную статистику на экран.
        /// </summary>
        /// <param name="args">Первый параметр - путь до первого файла.
        /// Второй параметр - путь до второго файла.</param>
        private static void Main(string[] args)
        {
            //TODO раскомментировать.
            //if (args.Length == 0)
            //{
            //    ConsoleHelper.Write("Args was empty");
            //    return;
            //}

            InitializeComponents();

            _statisticService.AnalyzingCompleted += StatisticService_AnalyzingCompleted;

            _statisticService.SetFilePath(@"C:\Users\mikhi\Desktop\dev\1.txt")
                .SetCharsTypeResulting(CharType.Vowel)
                .SetCompareCharsCount(1)
                .SetIgnoreCaseRequire(false)
                .StartAnalyzing();

            _statisticService.AnalyzingCompleted -= StatisticService_AnalyzingCompleted;

            ConsoleHelper.ReadKey();
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(IEnumerable<LetterStats> stats, bool isSortingNecessary)
        {
            if (isSortingNecessary)
            {
                stats = stats.OrderBy(stat => stat.Letter);
            }

            foreach (var stat in stats)
            {
                ConsoleHelper.WriteLine("{0} : {1}", stat.Letter, stat.Count);
            }

            ConsoleHelper.WriteLine("Letters count : {0 }", stats.Count());
        }

        /// <summary>
        /// Обрабатывает событие завершения анализа файла.
        /// </summary>
        /// <param name="sender">Обьект, который вызвал событие.</param>
        /// <param name="e">Параметры события.</param>
        private static void StatisticService_AnalyzingCompleted(object sender, AnalyzingCompletedEventArgs e)
        {
            PrintStatistic(e.Result, true);
        }

        #endregion Private Methods
    }
}