using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TestTask
{
    public class Program
    {

        /// <summary>
        /// Программа принимает на вход 2 пути до файлов.
        /// Анализирует в первом файле кол-во вхождений каждой буквы (регистрозависимо). Например А, б, Б, Г и т.д.
        /// Анализирует во втором файле кол-во вхождений парных букв (не регистрозависимо). Например АА, Оо, еЕ, тт и т.д.
        /// По окончании работы программа выводит данную статистику на экран
        /// </summary>
        /// <param name="args">Первый параметр - путь до первого файла, второй параметр - путь до второго файла</param>
        static void Main(string[] args)
        {
            try
            {
                string path1 = args[0].Trim();
                string path2 = args[1].Trim();
                if (!(File.Exists(path1) && File.Exists(path2)))
                    throw new Exception("Проверьте корректность введённых путей до файлов!!!");

                using (IReadOnlyStream inputStream1 = GetInputStream(path1))
                using (IReadOnlyStream inputStream2 = GetInputStream(path2))
                {
                    List<LetterStat> singleLetterStats = FillSingleLetterStats(inputStream1);
                    List<LetterStat> doubleLetterStats = FillDoubleLetterStats(inputStream2);

                    RemoveCharStatsByType(singleLetterStats, CharType.Vowels);
                    RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

                    PrintStatistic(singleLetterStats);
                    PrintStatistic(doubleLetterStats);
                }

            } catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }

            Console.WriteLine();
            Console.WriteLine("Для завершения работы программы нажмите клавишу Enter...");
            Console.ReadLine();

        }

        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения</returns>
        private static IReadOnlyStream GetInputStream(string fileFullPath)
        {
            return new ReadOnlyStream(fileFullPath);
        }

        /// <summary>
        /// Ф-ция, считывающая из входящего потока все буквы и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима</returns>
        private static List<LetterStat> FillSingleLetterStats(IReadOnlyStream stream)
        {
            var chars = GetCyrillicChars(stream);
            return chars.GroupBy(ch => ch).Select(g => new LetterStat
                {
                    Letter = g.Key.ToString(),
                    Count = g.Count()
                }).ToList();

        }

        /// <summary>
        /// Ф-ция, считывающая из входящего потока все буквы и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по одинаковым паросочетаниям букв, что были прочитаны из стрима</returns>
        private static List<LetterStat> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            var chars = GetCyrillicChars(stream);
            var text = new string(chars.ToArray());
            return Regex.Matches(text, Patterns.Duplicates, RegexOptions.IgnoreCase)
                .Cast<Match>().Select(ch => ch.Value.ToLower())
                .GroupBy(ch => ch).Select(g => new LetterStat
                    {
                        Letter = g.Key,
                        Count = g.Count()
                    }).ToList();

        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы, мягкий или твёрдый знак
        /// (тип букв для перебора определяется параметром charType).
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(List<LetterStat> letters, CharType charType)
        {
            letters.RemoveAll(let => GetCharType(let) == charType);            
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(IEnumerable<LetterStat> letters)
        {
            var count = 0;
            foreach (var let in letters.OrderBy(let => let.Letter))
            {
                count += let.Count;
                Console.WriteLine($"{let.Letter} : {let.Count}");
            }

            Console.WriteLine($"ИТОГО : {count}");
        }

        /// <summary>
        /// Посимвольно читает поток для отбора кириллических символов
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Список кириллических символов</returns>
        private static List<char> GetCyrillicChars(IReadOnlyStream stream)
        {
            var chars = new List<char>();

            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char ch = stream.ReadNextChar();

                if (!Regex.IsMatch(ch.ToString(), Patterns.Cyrillic))
                    continue;

                chars.Add(ch);
            }

            return chars;
        }

        /// <summary>
        /// Определяет тип букв
        /// </summary>
        /// <param name="letterStat">Статистика вхождения буквы/пары букв</param>
        /// <returns>Тип букв</returns>
        private static CharType GetCharType(LetterStat letterStat)
        {
            if (Regex.IsMatch(letterStat.Letter, Patterns.Consonants, RegexOptions.IgnoreCase))
                return CharType.Consonants;

            if (Regex.IsMatch(letterStat.Letter, Patterns.Vowels, RegexOptions.IgnoreCase))
                return CharType.Vowels;

            if (Regex.IsMatch(letterStat.Letter, Patterns.SoftSign, RegexOptions.IgnoreCase))
                return CharType.SoftSign;

            if (Regex.IsMatch(letterStat.Letter, Patterns.SolidSign, RegexOptions.IgnoreCase))
                return CharType.SolidSign;

            return CharType.Unidentified;

        }

    }
}
