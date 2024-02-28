using System;
using TestTask.Model;
using TestTask.View;

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
            ILetterCountModel letterContModel1 = new SingleLetterCountModel();
            ILetterCountView letterContView1 = new SingleCountView();
            ILetterStatisticsController letterStatisticsController1 = new LetterStatisticsController(letterContModel1, letterContView1);

            ILetterCountModel letterContModel2 = new PairLetterCountModel();
            ILetterCountView letterContView2 = new PairLetterCountView();
            ILetterStatisticsController letterStatisticsController2 = new LetterStatisticsController(letterContModel2, letterContView2);

            letterStatisticsController1.CalculateLetterStatistics(args[0]);
            letterStatisticsController1.RemoveCharStatsByType(CharType.Consonants);
            letterStatisticsController1.PrintStatistic();

            letterStatisticsController2.CalculateLetterStatistics(args[1]);
            letterStatisticsController2.RemoveCharStatsByType(CharType.Vowel);
            letterStatisticsController2.PrintStatistic();

            Console.Write("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}
