using System;
using System.Collections.Generic;
using TestTask.Enums;
using TestTask.Models;
using TestTask.Services;
using TestTask.Services.Base;
using TestTask.Services.Interfaces;

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
            IList<LetterStats> singleLetterStats;
            IList<LetterStats> doubleLetterStats;

            ReadOnlyStreamFactory factory = new ReadOnlyFileStreamFactory();

            using (IReadOnlyStream inputStream1 = factory.Create(args[0]))
            {
                singleLetterStats = StatisticsService.FillSingleLetterStats(inputStream1);
            }
            using (IReadOnlyStream inputStream2 = factory.Create(args[1])) 
            {
                doubleLetterStats = StatisticsService.FillDoubleLetterStats(inputStream2);
            }
            
            StatisticsService.RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            StatisticsService.RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            StatisticsService.PrintStatistic(singleLetterStats);
            StatisticsService.PrintStatistic(doubleLetterStats);

            // TODO : Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
        }
    }
}
