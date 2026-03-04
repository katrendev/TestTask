using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    /// <summary>
    /// Методы для работы с вводом/выводом
    /// </summary>
    internal class ConsoleHelperMethods
    {
        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        public static void PrintStatistic(IEnumerable<LetterStats> letters, int columnLength = 15)
        {
            IEnumerable<LetterStats> sortedLetters = letters.OrderBy(x => x.Letter);

            Console.WriteLine($"|{new string('-', columnLength)}|{new string('-', columnLength)}|");
            Console.WriteLine($"|{"Буква".PadRight(columnLength)}|{"Кол-во".PadRight(columnLength)}|");
            foreach (LetterStats letterData in sortedLetters)
            {
                Console.WriteLine($"|{new string('-', columnLength)}|{new string('-', columnLength)}|");
                string letterText = letterData.Letter;
                string countText = letterData.Count.ToString();

                Console.WriteLine($"|{letterText.PadRight(columnLength)}|{countText.PadRight(columnLength)}|");
            }

            Console.WriteLine($"|{new string('-', columnLength)}|{new string('-', columnLength)}|");
        }
    }
}
