using System;
using System.Collections.Generic;
using System.IO;

namespace TestTask
{
    public static class Program
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
            const int filesCount = 2;
            string[] files = CheckArgs(args, filesCount);

            IReadOnlyStream inputStream1 = new ReadOnlyStream(files[0]);
            IReadOnlyStream inputStream2 = new ReadOnlyStream(files[1]);

            IList<LetterStats> singleLetterStats = LetterStatistics.FillSingleLetterStats(inputStream1);
            IList<LetterStats> doubleLetterStats = LetterStatistics.FillDoubleLetterStats(inputStream2);

            inputStream1.CloseStream();
            inputStream2.CloseStream();

            LetterStatistics.RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            LetterStatistics.RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            LetterStatistics.PrintStatistic(singleLetterStats);
            LetterStatistics.PrintStatistic(doubleLetterStats);


            Console.ReadLine();
        }

        /// <summary>
        /// Ф-ция делает проверку на то, что аргументами поступило нужное кол-во строк.
        /// Иначе заставляет их ввести.
        /// </summary>
        /// <param name="args">Массив строк, который нужно проверить</param>
        /// <param name="сount">Кол-во нужных строк</param>
        /// <returns>Строки из args или полученные в теле ф-ции</returns>
        private static string[] CheckArgs(string[] args, int count)
        {
            string[] files = new string[count];

            if (args.Length != count)
            {
                Console.WriteLine("Enter the file names:");
                for (int i = 0; i < count; i++)
                {
                    files[i] = Console.ReadLine();
                }
            }
            else
            {
                files = args;
            }

            return files;
        }
    }
}