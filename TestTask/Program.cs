using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    public class Program
    {
        private static readonly Dictionary<CharType, string> AlphabetChars =
            new Dictionary<CharType, string>
            {
                [CharType.Vowel] = "аеёиоуыэюя",
                [CharType.Consonants] = "бвгджзйклмнпрстфхцчшщ"
            };

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
            if (args.Length < 2)
            {
                throw new Exception("Количество путей к файлам меньше 2-х");
            }

            using (var inputStream1 = GetInputStream(args[0]))
            {
                var singleLetterStats = FillSingleLetterStats(inputStream1);
                var filteredStats = GetWithoutStatsByCharType(singleLetterStats, CharType.Vowel);

                PrintStatistic(filteredStats);
            }

            Console.WriteLine();

            using (var inputStream2 = GetInputStream(args[1]))
            {
                var doubleLetterStats = FillDoubleLetterStats(inputStream2);
                var filteredStats = GetWithoutStatsByCharType(doubleLetterStats, CharType.Consonants);

                PrintStatistic(filteredStats);
            }

            Console.WriteLine("Нажмите любую клавишу для выхода");
            Console.ReadKey();
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

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            var dict = new Dictionary<string, LetterStats>();

            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                var c = stream.ReadNextChar().ToString();
                dict.AddToDictionary(c);
            }

            return dict.Values.ToList();
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            var dict = new Dictionary<string, LetterStats>();
            char? prevChar = null;

            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                var currentChar = char.ToLower(stream.ReadNextChar());
                if (currentChar == prevChar)
                {
                    dict.AddToDictionary($"{prevChar}{currentChar}");
                }

                prevChar = currentChar;
            }

            return dict.Values.ToList();
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Возвращает все буквы/пары отличные параметру поиска
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static IList<LetterStats> GetWithoutStatsByCharType(IList<LetterStats> letters, CharType charType)
        {
            var charsForSearch = AlphabetChars[charType];
            return letters.Where(l => !IsLetterMatch(l.Letter)).ToList();


            bool IsLetterMatch(string letter)
            {
                return letter.ToLower().Any(charsForSearch.Contains);
            }
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            letters
                .OrderBy(l => l.Letter)
                .ToList()
                .ForEach(ls => Console.WriteLine($"{ls.Letter} : {ls.Count}"));

            Console.WriteLine($"Итого: {letters.Sum(l=> l.Count)}");
        }
    }
}
