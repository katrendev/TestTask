using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            if (args.Length == 2)
                try
                {
                    using (var inputStream1 = GetInputStream(args[0]))
                    {
                        LetterStatMaker singleLetterStats = new SingleLetterStatMaker(inputStream1);
                        singleLetterStats.RemoveCharStatsByType(CharType.Vowel);

                        Console.WriteLine($"singleLetterStats ('{args[0]}'):");
                        singleLetterStats.PrintStatistic();
                    }
                    Console.WriteLine("");

                    using (var inputStream2 = GetInputStream(args[1]))
                    {
                        LetterStatMaker doubleLetterStats = new DoubleLetterStatMaker(inputStream2);
                        doubleLetterStats.RemoveCharStatsByType(CharType.Consonants);

                        Console.WriteLine($"doubleLetterStats ('{args[1]}'):");
                        doubleLetterStats.PrintStatistic();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            else Console.WriteLine("Параметров должно быть 2. Первый параметр - путь до первого файла. Второй параметр - путь до второго файла.");

            Console.ReadKey();
        }

        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <param name="encoding">Кодировка (по умолчанию - автоопределение или UTF-8)</param>
        /// <returns>Поток для последующего чтения.</returns>
        private static IReadOnlyStream GetInputStream(string fileFullPath,Encoding encoding=null)
        {
            return new ReadOnlyStream(fileFullPath,encoding);
        }

      


       

       

      


    }
}
