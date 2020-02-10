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

            Statistics doubleLetterStatistic = new DoubleLetterStatistics(args[0]);
            doubleLetterStatistic.RemoveCharStatsByType(CharType.Vowel);
            doubleLetterStatistic.PrintStatistic();
            Statistics singleLetterStatistics = new SingleLetterStatistics(args[1]);
            singleLetterStatistics.RemoveCharStatsByType(CharType.Consonants);
            singleLetterStatistics.PrintStatistic();
            Console.Read();
        }
    }
}
