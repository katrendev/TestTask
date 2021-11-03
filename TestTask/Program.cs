using System;
using System.Collections.Generic;
using System.IO;
using TestTask.Helpers;
using TestTask.Interfaces;
using TestTask.Models;
using TestTask.Services;

namespace TestTask
{
    public class Program
    {
        #region Private Methods

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
            var statisticService = new TextStatisticService();

            try
            {
                string firstTestFilePath = args[0];
                string secondTestFilePath = args[1];

                //Выполнение первой задачи приложения.
                using (IReadOnlyStream singleCharInputStream = statisticService.GetInputStream(secondTestFilePath))
                {
                    IList<LetterStats> singleLetterStats =
                        statisticService.GetSymbolStatistic(singleCharInputStream, statisticService.FillSingleLetterStats);

                    statisticService.RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                    statisticService.PrintStatistic(singleLetterStats, 1);
                }

                //Выполнение второй задачи приложения.
                using (IReadOnlyStream doubleCharInputStream = statisticService.GetInputStream(firstTestFilePath))
                {
                    IList<LetterStats> doubleLetterStats =
                        statisticService.GetSymbolStatistic(doubleCharInputStream, statisticService.FillDoubleLetterStats);

                    statisticService.RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);
                    statisticService.PrintStatistic(doubleLetterStats, 2);
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine($"Произошла ошибка связанная с файловой директроие: {ex.Message}");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Произошла ошибка связанная с доступом к файлу: {ex.Message}");
            }
            catch (OverflowException ex)
            {
                Console.WriteLine($"Произошло ошибка при чтении файла: {ex.Message}");
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine($"Не были переданны пути до файлов.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла критическая: {ex.Message}");
            }

            // TODO : Необходимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
            Console.ReadKey();
        }

        #endregion Private Methods
    }
}
