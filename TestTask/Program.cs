using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestTask.Io;
using TestTask.Stats;
using TestTask.Util;

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
            if (args.Length < 2)
            {
                Console.WriteLine("Использовать так: TestTask.exe <first_file_name> <second_file_name>");
                return;
            }

            IList<LetterStatItem> singleLetterStats;
            IList<LetterStatItem> doubleLetterStats;

            try
            {
                using (var stream = GetInputStream(args[0]))
                {
                    singleLetterStats = FillSingleLetterStats(stream);
                }

                using (var stream = GetInputStream(args[1]))
                {
                    doubleLetterStats = FillDoubleLetterStats(stream);
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Неожиданная ошибка: {e.Message}");
                return;
            }

            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonant);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

            Console.Write("Нажмите любую кнопку для продолжения...");
            Console.ReadKey();
            Console.WriteLine();
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
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStatItem> FillSingleLetterStats(IReadOnlyStream stream)
        {
            stream.ResetPositionToStart();
            return LetterStatsCollector.Instance.Collect(stream);
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStatItem> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            stream.ResetPositionToStart();
            return LetterStatsCollectorPairs.Instance.Collect(stream);
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(IList<LetterStatItem> letters, CharType charType)
        {
            var toRemove = letters
                .Where(item => charType.Matches(item.Letter))
                .ToList();

            foreach (var item in toRemove)
                letters.Remove(item);
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(IEnumerable<LetterStatItem> letters)
        {
            var total = 0;
            foreach (var item in letters.OrderBy(item => item.Letter))
            {
                Console.WriteLine($"{item.Letter} : {item.Count}");
                total += item.Count;
                // Сумму можно было бы посчитать через letters.Sum(item => item.Count), но IEnumerable
                // не всегда можно итерировать больше одного раза, поэтому обезопасим себя таким образом
            }

            Console.WriteLine($"ИТОГО : {total}");
        }
    }
}