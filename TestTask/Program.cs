using System;
using System.Collections.Generic;
using System.Linq;

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
            try
            {
                IReadOnlyStream inputStream1 = ReadOnlyStream.GetInputStream(args[0]);
                IReadOnlyStream inputStream2 = ReadOnlyStream.GetInputStream(args[1]);

                var singleLetterStats = new ReadStream(new FillSingleLetterStats()).Read(inputStream1);
                var doubleLetterStats = new ReadStream(new FillDoubleLetterStats()).Read(inputStream2);

                LetterContext singleL = new LetterContext(singleLetterStats);
                LetterContext doubleL = new LetterContext(doubleLetterStats);

                singleL.RemoveCharStatsByType(CharType.Consonants);
                doubleL.RemoveCharStatsByType(CharType.Vowel);

                singleL.PrintStatistic();
                doubleL.PrintStatistic();
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine(e.Message+ " " + nameof(args));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.ReadKey();
        }
    }
}