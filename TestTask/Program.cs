using System;
using System.Linq;
using System.Collections.Generic;

using TestTask.Stream;
using TestTask.Extensions;
using TestTask.ValueTypes;

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
        private static void Main(string[] args)
        {
            IReadOnlyStream inputStream1;
            IReadOnlyStream inputStream2;
#if DEBUG
            inputStream1 = GetInputStream(Samples.Single());
            inputStream2 = GetInputStream(Samples.Double());
#else
            inputStream1 = GetInputStream(args[0]);
            inputStream2 = GetInputStream(args[1]);
#endif
            var singleLetterStats = FillSingleLetterStats(inputStream1);
            var doubleLetterStats = FillDoubleLetterStats(inputStream2);

            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

            Console.Write("Нажмите любую клавишу для выхода..."); // as C++: system("pause");
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
            stream.ResetPositionToStart();
            Dictionary<char, int> charCount = new Dictionary<char, int>();

            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (char.IsLetter(c))
                {
                    char letter = char.ToUpper(c);
                    if (!charCount.ContainsKey(letter))
                        charCount[letter] = 0;
                    charCount[letter]++;
                }
            }

            return charCount.Select(pair => new LetterStats { Letter = pair.Key, Count = pair.Value }).ToList();
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
            stream.ResetPositionToStart();
            Dictionary<string, int> charPairCount = new Dictionary<string, int>();

            char prevChar = '\0';   
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if ((char.IsLetter(prevChar) && char.IsLetter(c)) && prevChar == c)
                {
                    string charPair = string.Concat(char.ToUpper(prevChar), char.ToUpper(c));
                    if (!charPairCount.ContainsKey(charPair))
                        charPairCount[charPair] = 0;
                    charPairCount[charPair]++;
                }
                prevChar = c;
            }

            return charPairCount.Select(pair => new LetterStats { Letter = pair.Key[0], Count = pair.Value }).ToList();
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            letters.RemoveAll(stat => (charType == CharType.Vowel && stat.Letter.IsConsonant()) ||
                                      (charType == CharType.Consonants && stat.Letter.IsVowel()));
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
            var letterStatsEnumerable = letters as LetterStats[] ?? letters.ToArray();
            foreach (var letterStat in letterStatsEnumerable.OrderBy(stat => stat.Letter))
            {
                Console.WriteLine($"{letterStat.Letter} : {letterStat.Count}");
            }

            int totalCount = letterStatsEnumerable.Sum(stat => stat.Count);
            Console.WriteLine($"Итого: {totalCount}");
            Console.WriteLine();
        }
    }
}
