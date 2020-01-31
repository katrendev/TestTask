using System;
using TestTask.Models;
using TestTask.Services;

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
            IReadOnlyStream inputStream1 = GetInputStream(args[0]);
            IReadOnlyStream inputStream2 = GetInputStream(args[1]);

            var singleLetterStats = LettersAnalyzer.FillSingleLetterStats(inputStream1);
            var doubleLetterStats = LettersAnalyzer.FillDoubleLetterStats(inputStream2);

            LettersAnalyzer.RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            LettersAnalyzer.RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            LettersAnalyzer.PrintStatistic(singleLetterStats);
            LettersAnalyzer.PrintStatistic(doubleLetterStats);

			Console.Read();
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
    }
}
