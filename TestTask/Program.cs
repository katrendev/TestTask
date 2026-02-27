using System;
using System.Collections.Generic;
using TestTask.Extensions;
using TestTask.Interfaces;
using TestTask.Models;
using TestTask.Services;
using TestTask.Services.LetterStrategy;

namespace TestTask
{
    internal static class Program
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
                Console.WriteLine("Использование: TestTask <путь_к_файлу1> <путь_к_файлу2>");
                return;
            }

            try
            {
                IList<LetterStats> singleLetterStats;
                IList<LetterStats> doubleLetterStats;
                
                using (IReadOnlyStream inputStream1 = new ReadOnlyStream(args[0]))
                {
                    singleLetterStats = LetterStatsService.FillSingleLetterStats(inputStream1);
                }
                using (IReadOnlyStream inputStream2 = new ReadOnlyStream(args[1]))
                {
                    doubleLetterStats = LetterStatsService.FillDoubleLetterStats(inputStream2);
                }

                RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

                LetterStatisticsPrintHelper.PrintStatisticSorted(singleLetterStats);
                LetterStatisticsPrintHelper.PrintStatisticSorted(doubleLetterStats);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"Файл не найден: {e.FileName}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка: {e.Message}");
            }
            finally
            {
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            var strategy = CharFilterStrategyFactory.GetStrategy(charType);
            letters.RemoveElementsByStrategy(strategy);
        }

    }
}
