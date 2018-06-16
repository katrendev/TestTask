using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestTask
{
    public class Program
    {
        /// <summary>
        ///     Программа принимает на входе 2 пути до файлов.
        ///     Анализирует в первом файле кол-во вхождений каждой буквы (регистрозависимо). Например А, б, Б, Г и т.д.
        ///     Анализирует во втором файле кол-во вхождений парных букв (не регистрозависимо). Например АА, Оо, еЕ, тт и т.д.
        ///     По окончанию работы - выводит данную статистику на экран.
        /// </summary>
        /// <param name="args">
        ///     Первый параметр - путь до первого файла.
        ///     Второй параметр - путь до второго файла.
        /// </param>
        public static void Main(string[] args)
        {
            var inputStream1 = default(IReadOnlyStream);
            var inputStream2 = default(IReadOnlyStream);

            try
            {
                inputStream1 = GetInputStream(args[0]);
                inputStream2 = GetInputStream(args[1]);

                var singleLetterStats = LetterStatsWrapper.FillSingleLetterStats(inputStream1).ToList();
                var doubleLetterStats = LetterStatsWrapper.FillDoubleLetterStats(inputStream2).ToList();

                LetterStatsWrapper.RemoveCharStatsByType(singleLetterStats, LetterType.Vowel);
                LetterStatsWrapper.RemoveCharStatsByType(doubleLetterStats, LetterType.Consonant);

                PrintStatistic(singleLetterStats, "Single Letter Stats");
                PrintStatistic(doubleLetterStats, "Double Letter Stats");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                inputStream1?.Close();
                inputStream2?.Close();
            }

            Console.WriteLine("Press Any Key To Exit...");
            Console.ReadKey();
        }

        /// <summary>
        ///     Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        private static IReadOnlyStream GetInputStream(string fileFullPath)
        {
            if (!File.Exists(fileFullPath))
            {
                throw new FileNotFoundException("File not found", fileFullPath);
            }

            return new ReadOnlyStream(new FileStream(fileFullPath, FileMode.Open, FileAccess.Read));
        }

        /// <summary>
        ///     Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        ///     Каждая буква - с новой строки.
        ///     Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        ///     В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        /// <param name="preambula">Заголовок статистики</param>
        private static void PrintStatistic(IEnumerable<LetterStats> letters, string preambula)
        {
            Console.WriteLine(preambula);
            var total = 0;
            var sortedStats = letters.OrderBy(stats => char.ToLower(stats.Letter));

            foreach (var letter in sortedStats)
            {
                Console.WriteLine($"{letter.Letter} : {letter.Count}");
                total += letter.Count;
            }

            Console.WriteLine($"Total: {total}");
        }
    }
}