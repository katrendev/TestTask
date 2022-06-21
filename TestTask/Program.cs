using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTask.Data.English;
using TestTask.Enums;
using TestTask.Helpers;
using TestTask.Models;
using TestTask.Streams;
using TestTask.Streams.Interfaces;

namespace TestTask
{
    public class Program
    {
        #region Private Methods

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IEnumerable<LetterStats<string>> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            IDictionary<string, int> statistic = new Dictionary<string, int>();

            stream.ResetPositionToStart();

            StringBuilder doubleLetters = new StringBuilder();

            if (!stream.IsEof)
            {
                doubleLetters.Append(stream.ReadNextChar());
            }

            while (!stream.IsEof)
            { 
                ///TODO проверка на конец файла.
                doubleLetters.Append(stream.ReadNextChar());

                bool isDoubledLetters = string.Equals(doubleLetters[0].ToString(), doubleLetters[1].ToString(), StringComparison.OrdinalIgnoreCase);

                if (isDoubledLetters)
                {
                    string doubledLettersString = doubleLetters.ToString();

                    if (statistic.ContainsKey(doubledLettersString))
                    {
                        statistic[doubledLettersString]++;
                    }
                    else
                    {
                        statistic.Add(doubledLettersString, 1);
                    }
                }

                doubleLetters.Remove(0, 1);
            }

            return CreateStats(statistic);
        }

        /// <summary>
        /// Создает статистику из <see cref="Dictionary{T, int}"/>
        /// </summary>
        /// <typeparam name="T">Тип ключа словаря.</typeparam>
        /// <param name="stats">Статистика в виде словаря.</param>
        /// <returns>Статистика в виде <see cref="List{LetterStats}"/>.</returns>
        private static IEnumerable<LetterStats<T>> CreateStats<T>(IDictionary<T, int> stats)
        {
            foreach (var stat in stats)
            {
                yield return new LetterStats<T>(stat.Key, stat.Value);
            }
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IEnumerable<LetterStats<char>> FillSingleLetterStats(IReadOnlyStream stream)
        {
            IDictionary<char, int> statistic = new Dictionary<char, int>();
            stream.ResetPositionToStart();

            while (!stream.IsEof)
            {
                char nextChar = stream.ReadNextChar();

                if (statistic.ContainsKey(nextChar))
                {
                    statistic[nextChar]++;
                }
                else
                {
                    statistic.Add(nextChar, 1);
                }
            }

            return CreateStats(statistic);
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
        /// Программа принимает на входе 2 пути до файлов.
        /// Анализирует в первом файле кол-во вхождений каждой буквы (регистрозависимо). Например А, б, Б, Г и т.д.
        /// Анализирует во втором файле кол-во вхождений парных букв (не регистрозависимо). Например АА, Оо, еЕ, тт и т.д.
        /// По окончанию работы - выводит данную статистику на экран.
        /// </summary>
        /// <param name="args">Первый параметр - путь до первого файла.
        /// Второй параметр - путь до второго файла.</param>
        private static void Main(string[] args)
        {
            //TODO раскомментировать.
            //if (args.Length == 0)
            //{
            //    ConsoleHelper.Write("Args was empty");
            //    return;
            //}

            IEnumerable<LetterStats<char>> singleLetterStats;

            using (IReadOnlyStream inputStream1 = GetInputStream(@"C:\Users\mikhi\Desktop\dev\1.txt"))
            {
                singleLetterStats = FillSingleLetterStats(inputStream1);
            }

            IEnumerable<LetterStats<string>> doubleLetterStats;

            using (IReadOnlyStream inputStream1 = GetInputStream(@"C:\Users\mikhi\Desktop\dev\1.txt"))
            {
                doubleLetterStats = FillDoubleLetterStats(inputStream1);
            }

            //IReadOnlyStream inputStream1 = GetInputStream(args[0]);
            //IReadOnlyStream inputStream2 = GetInputStream(args[1]);

            singleLetterStats = RemoveCharStatsByType(singleLetterStats, CharType.Consonants);
            doubleLetterStats = RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

            ConsoleHelper.ReadKey();
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic<T>(IEnumerable<LetterStats<T>> stats, bool isSortingNecessary = true)
        {
            if (isSortingNecessary)
            {
                stats = stats.OrderBy(stat => stat.Letter);
            }     

            foreach (var stat in stats)
            {
                ConsoleHelper.WriteLine("{0} : {1}", stat.Letter, stat.Count);
            }

            ConsoleHelper.WriteLine("Letters count : {0 }", stats.Count());
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static IEnumerable<LetterStats<T>> RemoveCharStatsByType<T>(IEnumerable<LetterStats<T>> statistic, CharType charType)
        {
            string charsFilter = null;

            switch (charType)
            {
                case CharType.Consonants:
                    charsFilter = ListOfCharsTypes.ConsonantsChars;
                    break;

                case CharType.Vowel:
                    charsFilter = ListOfCharsTypes.VovelChars;
                    break;

                default:
                    charsFilter = ListOfCharsTypes.ConsonantsChars;
                    break;
            }

            foreach (var letterStat in statistic)
            {
                char letterToCompare = letterStat.Letter.ToString()[0];

                if (charsFilter.Contains(letterToCompare))
                {
                    yield return letterStat;
                }
            }
        }

        #endregion Private Methods
    }
}