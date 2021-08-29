using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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
        static async Task Main(string[] args)
        {
            if (CheckArgs(args))
            {
                return;
            }

            using (IReadOnlyStream inputStream1 = await ReadOnlyStream.GetInputStreamAsync(args[0]),
                                   inputStream2 = await ReadOnlyStream.GetInputStreamAsync(args[1]))
            {
                IList<LetterStats> singleLetterStats = await Statistics.FillSingleLetterStatsAsync(inputStream1);
                IList<LetterStats> doubleLetterStats = await Statistics.FillDoubleLetterStatsAsync(inputStream2);

                singleLetterStats = await Statistics.RemoveCharStatsByTypeAsync(singleLetterStats, CharType.Vowels);
                doubleLetterStats = await Statistics.RemoveCharStatsByTypeAsync(doubleLetterStats, CharType.Consonants);

                await Statistics.PrintStatisticsAsync(singleLetterStats);
                await Statistics.PrintStatisticsAsync(doubleLetterStats);
            }

            // TODO : Необходимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
            DisplayExitMessage();
        }

        #region Private Helpers

        private static void DisplayExitMessage()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }

        private static void DisplayArgsNumberMessage()
        {
            Console.WriteLine("");
            Console.WriteLine("You must provide two file names!");
            Console.WriteLine("The program will terminate.");
        }

        private static void DisplayFileErrorMessage()
        {
            Console.WriteLine("");
            Console.WriteLine("One or more files could not be found!");
            Console.WriteLine("The program will terminate.");
        }

        private static bool CheckArgs(string[] args)
        {
            bool terminate = false;

            if (args.Length != 2)
            {
                DisplayArgsNumberMessage();
                DisplayExitMessage();

                terminate = true;
            }
            else if (!File.Exists(args[0]) || !File.Exists(args[1]))
            {
                DisplayFileErrorMessage();
                DisplayExitMessage();

                terminate = true;
            }

            return terminate;
        }

        #endregion
    }
}
