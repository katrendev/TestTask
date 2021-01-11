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
            if(!args.Any() )
            {
                Console.WriteLine("Arguments wrong!");
                return;
            }

            if (string.IsNullOrEmpty(args[0]) || string.IsNullOrEmpty(args[1]))
            {
                Console.WriteLine("Arguments wrong!");
                return;
            }


            try
            {
                IReadOnlyStream leftStream = new ReadOnlyStream(args[0]);
                IReadOnlyStream rigthStream = new ReadOnlyStream(args[1]);

                // Implementation a logic
                IStatisticsCreator parsing = new StatisticsCreator();
                var leftResult = parsing.FillSingleLetterStats(leftStream);
                var rightResult = parsing.FillDoubleLetterStats(rigthStream);

                // Printing a results
                PrintStatistic(leftResult);
                Console.WriteLine("=================================================");
                PrintStatistic(rightResult);
                Console.WriteLine("=================================================");
                

                var parserObject = parsing as StatisticsCreator;
                if(parserObject != null)
                {
                    Console.WriteLine("Excluded A symbol from rightResult");
                    var excludedList = new List<string>() { "A" };
                    PrintStatistic(parserObject.RemoveCharStatsByType(rightResult, excludedList));

                    Console.WriteLine("=================================================");
                    Console.WriteLine("Excluded a Consonants symbols from rightResult");
                    PrintStatistic(parserObject.RemoveCharStatsByType(rightResult, CharType.Consonants));
                }
                
                // The end 8-)
            }
            catch (Exception ex)
            {
                Console.WriteLine("============ ERROR ===============");
                Console.WriteLine(ex.Message + ex.InnerException?.Message);
            }

            Console.WriteLine("=================================================");
            Console.WriteLine("Press any keys");
            Console.ReadLine();
        }
      

     

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(IEnumerable<LetterStats>  letters)
        {
            if (letters == null) return;

            if(letters.Any())
            {
                letters.OrderBy(x => x.Letter)
                    .ToList()
                    .ForEach(x =>
                    {
                        Console.WriteLine($"{x.Letter}|{x.Count}\n");
                    });

                Console.WriteLine("======== TOTAL ================");
                Console.WriteLine($"Keys: {letters.Count()}|Score:{letters.Sum(y => y.Count)}");
            }
        }
    }
}
