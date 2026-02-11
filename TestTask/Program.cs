using System.Collections.Generic;
using TestTask.Helpers;

namespace TestTask
{
    public class Program
    {
        /// <summary>
        /// Программа принимает на входе 2 пути до файлов.
        /// Анализирует в первом файле кол-во вхождений каждой буквы (регистрозависимо). Например А, б, Б, Г и т.д.
        /// Анализирует во втором файле кол-во вхождений парных букв (не регистрозависимо). Например АА, Оо, еЕ, тт и т.д.
        /// По окончанию работы - выводит данную статистику на экран.
        /// </summary>
        /// <param name="args">Первый параметр - путь до первого файла.
        /// Второй параметр - путь до второго файла.</param>
        static void Main(string[] args)
        {
            if (HelperMain.IsBadArgs(args))
            {
                HelperMain.PrintHelp();
            }
            else
            {
                ProcessingSingleStats(args[0]);
                ProcessingDoubleStats(args[1]);
            }
            // TODO : Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы. (+)
            HelperMain.WaitPressKey();
        }

        /// <summary>
        /// Метод запускает обработку файла для одиночных букв.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        private static void ProcessingSingleStats(string fileFullPath)
        {
            var singleStats = GetSingleLetterStats(fileFullPath);
            HelperRemoveStat.RemoveCharStatsByType(singleStats, CharType.Vowel);
            PrintStatistic(singleStats);
        }

        /// <summary>
        /// Метод запускает обработку файла для парных букв.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        private static void ProcessingDoubleStats(string fileFullPath)
        {
            var doubleStats = GetDoubleLetterStats(fileFullPath);
            HelperRemoveStat.RemoveCharStatsByType(doubleStats, CharType.Consonants);
            PrintStatistic(doubleStats);
        }

        /// <summary>
        /// Метод получает статистику по одиночным буквам.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Коллекция статистик.</returns>
        private static IList<LetterStats> GetSingleLetterStats(string fileFullPath)
        {
            IList<LetterStats> singleLetterStats = new List<LetterStats>();
            using (IReadOnlyStream inputStream = GetInputStream(fileFullPath))
            {
                singleLetterStats = HelperFillStat.FillSingleLetterStats(inputStream);
            }
            return singleLetterStats;
        }

        /// <summary>
        /// Метод получает статистику по парным буквам.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Коллекция статистик.</returns>
        private static IList<LetterStats> GetDoubleLetterStats(string fileFullPath)
        {
            IList<LetterStats> doubleLetterStats = new List<LetterStats>();
            using (IReadOnlyStream inputStream = GetInputStream(fileFullPath))
            {
                doubleLetterStats = HelperFillStat.FillDoubleLetterStats(inputStream);
            }
            return doubleLetterStats;
        }

        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        private static IReadOnlyStream GetInputStream(string fileFullPath)
        {
            return new ReadOnlyStream(fileFullPath);
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        public static void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту! (+)
            var report = HelperPrintStat.BuildReport(letters);
            HelperMain.PrintToConsole(report);
        }
    }
}
