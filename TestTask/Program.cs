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
            if (args.Length < 2)
            {
                ConsoleHelper.WriteLine("It needs 2 arguments to work.");
                return;
            }

            InitializeComponents();

            _statisticService.AnalyzingCompleted += StatisticService_AnalyzingCompleted;

            //Анализ 1-го файла.
            string firstFilePath = args[0];          
            _statisticService.SetFilePath(firstFilePath)
                .SetCharsTypeResulting(CharType.Vowel)
                .SetCompareCharsCount(1)
                .SetIgnoreCaseRequire(false)
                .StartAnalyzing();

            //Анализ 2-го файла.
            string secondFilePath = args[1];
            _statisticService.SetFilePath(secondFilePath)
                .SetCharsTypeResulting(CharType.Consonants)
                .SetCompareCharsCount(2)
                .SetIgnoreCaseRequire(true)
                .StartAnalyzing();

            _statisticService.AnalyzingCompleted -= StatisticService_AnalyzingCompleted;

            ConsoleHelper.ReadKey();
        }

        /// <summary>
        /// Выводит на экран статистику анализа файла.
        /// </summary>
        /// <param name="stats">Статистика для вывода.</param>
        /// <param name="isSortingNecessary">Необходимо ли сортировать статистику.</param>
        private static void PrintStatistic(IEnumerable<EntryStats> stats, bool isSortingNecessary)
        {
            if (isSortingNecessary)
            {
                stats = stats.OrderBy(stat => stat.Entry);
            }

            foreach (var stat in stats)
            {
                ConsoleHelper.WriteLine("{0} : {1}", stat.Entry, stat.Count);
            }

            ConsoleHelper.WriteLine("Entries count : {0 }", stats.Count());
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