using System;
using System.Collections.Generic;
using System.Linq;
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
        private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.
            }

            //return ???;

            throw new NotImplementedException();
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static Dictionary<char, int> FillSingleLetterStats(IReadOnlyStream stream)
        {
            Dictionary<char, int> statistic = new Dictionary<char, int>();

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

            return statistic;
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
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats">Статистика вхождений по букве/паре букв.</param>
        private static void IncStatistic(LetterStats letterStats)
        {
            letterStats.IncreaseCount();
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
            //if (args.Length == 0)
            //{
            //    ConsoleHelper.Write("Args was empty");
            //    return;
            //}

            Dictionary<char, int> singleLetterStats;

            using (IReadOnlyStream inputStream1 = GetInputStream(@"C:\Users\mikhi\Desktop\dev\1.txt"))
            {
                singleLetterStats = FillSingleLetterStats(inputStream1);
            }

            //IReadOnlyStream inputStream1 = GetInputStream(args[0]);
            //IReadOnlyStream inputStream2 = GetInputStream(args[1]);

            //IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

            RemoveCharStatsByType(singleLetterStats, CharType.Consonants);
            //RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            //PrintStatistic(doubleLetterStats);

            ConsoleHelper.ReadKey();
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(Dictionary<char, int> letters)
        {
            // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!

            IOrderedEnumerable<KeyValuePair<char, int>> sortedDictionary = from entry in letters orderby entry.Key ascending select entry;

            foreach (var entries in sortedDictionary)
            {
                ConsoleHelper.WriteLine("{0} : {1}", entries.Key, entries.Value);
            }

            ConsoleHelper.WriteLine("Letters count : {0 }", letters.Keys.Count());
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(Dictionary<char, int> statistic, CharType charType)
        {
            IReadOnlyList<char> charsFilter = null;

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

            foreach (var letterStat in statistic.ToDictionary(key => key.Key, value => value.Value))
            {
                if (!charsFilter.Contains(letterStat.Key))
                {
                    statistic.Remove(letterStat.Key);
                }
            }
        }

        #endregion Private Methods
    }
}