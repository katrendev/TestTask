using System;
using System.Collections.Generic;
using System.IO;

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
                IReadOnlyStreamFactory factory = new BaseReadOnlyStreamFactory();
                LetterAnalysisFacade facade = new LetterAnalysisFacade();

                using (IReadOnlyStream inputStream1 = factory.GetInputStream(args[0]))
                {
                    IList<LetterStats> singleLetterStats = facade.FillSingleLetterStats(inputStream1);
                    singleLetterStats = facade.RemoveCharStatsByType(singleLetterStats, CharType.Vowel);

                    Console.WriteLine($"Результаты анализа файла {args[0]}\n");
                    ConsoleHelperMethods.PrintStatistic(singleLetterStats);
                }

                Console.WriteLine("\n");

                using (IReadOnlyStream inputStream2 = factory.GetInputStream(args[1]))
                {
                    IList<LetterStats> doubleLetterStats = facade.FillDoubleLetterStats(inputStream2);
                    doubleLetterStats = facade.RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

                    Console.WriteLine($"Результаты анализа файла {args[1]}\n");
                    ConsoleHelperMethods.PrintStatistic(doubleLetterStats);
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine($"Ошибка, передано {args.Length} аргументов командой строки вместо 2!");
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Возникла непредвиденная ошибка! \nТекст ошибки: " + ex.ToString());
            }

            Console.WriteLine("\n");
            Console.Write("Нажмите на любую кнопку, чтобы закрыть программу...");
            Console.ReadKey();
        }
    }
}
