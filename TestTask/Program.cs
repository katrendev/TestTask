using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace TestTask
{
    public class Program
    {
        const string Consonants = "[йцкнгшщзхфвпрлджчсмтьбЙЦУНГШЩЗХФВПРЛДЖЧСМИЬБqwrtpsdfghjklzxcvbnmQWERPSDFGHJKLZXCVBNM]";
        const string Vowels = "[уеыаоэяиюУУЫАОЭЯИЮaeiouyAEIOUY]";

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
                if (args.Length != 2)
                {
                    throw new Exception("Incorrect arguments number.");
                }

                using (IReadOnlyStream inputStream1 = GetInputStream(args[0]))
                using (IReadOnlyStream inputStream2 = GetInputStream(args[1]))
                {
                    LetterAnalyzer analyzer = new LetterAnalyzer();

                    IList<LetterStats> singleLetterStats = analyzer.FillSingleLetterStats(inputStream1);
                    IList<LetterStats> doubleLetterStats = analyzer.FillDoubleLetterStats(inputStream2);

                    RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                    RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

                    PrintStatistic(singleLetterStats);
                    PrintStatistic(doubleLetterStats);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        public static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            switch (charType)
            {
                case CharType.Consonants:

                    for (int i = letters.Count - 1; i >= 0; i--)
                    {
                        var stat = letters[i];
                        if (Regex.IsMatch(stat.Letter, Consonants))
                        {
                            letters.RemoveAt(i);
                        }
                    }

                    break;
                case CharType.Vowel:
                    

                    for (int i = letters.Count - 1; i >= 0; i--)
                    {
                        var stat = letters[i];
                        if (Regex.IsMatch(stat.Letter, Vowels))
                        {
                            letters.RemoveAt(i);
                        }
                    }

                    break;
            }

        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        public static void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            var sortedLetters = letters.OrderBy(l => l.Letter);

            Console.WriteLine($"Буква:\tКол-во");
            int finalCount = 0;
            foreach (var letter in sortedLetters)
            {
                Console.WriteLine($"{letter.Letter[0]}:\t{letter.Count}");
                finalCount += letter.Count;
            }

            Console.WriteLine($"Итого:\t{finalCount}");
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
