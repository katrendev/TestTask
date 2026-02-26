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
        /// <param name="args">Массив аргументов командной строки:
        /// args[0] - путь до первого файла,
        /// args[1] - путь до второго файла.</param>
        static void Main(string[] args)
        {
            IReadOnlyStream singleCharStream = GetInputStream(args[0]);
            IReadOnlyStream doubleCharStream = GetInputStream(args[1]);

            IList<CharStats> singleCharStats = CollectSingleCharStats(singleCharStream);
            IList<CharStats> doubleCharStats = CollectDoubleCharStats(doubleCharStream);

            RemoveCharStatsByType(singleCharStats, CharType.Vowel);
            RemoveCharStatsByType(doubleCharStats, CharType.Consonants);

            PrintStatistic(singleCharStats);
            PrintStatistic(doubleCharStats);

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
        private static IList<CharStats> CollectSingleCharStats(IReadOnlyStream stream)
        {
            var stats = new List<CharStats>();

            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char currentChar = stream.ReadNextChar();

                if (!char.IsLetter(currentChar))
                {
                    continue;
                }

                var stat = stats.FirstOrDefault(stat => stat.Char == currentChar.ToString());

                if (stat == null)
                {
                    stats.Add(new CharStats { Char = currentChar.ToString(), Count = 1 });
                }

                else
                {
                    IncStatistic(stat);
                }

            }
            return stats;
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<CharStats> CollectDoubleCharStats(IReadOnlyStream stream)
        {
            var stats = new List<CharStats>();
            bool prevWasLetter = false;
            char prevChar = default;

            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char currentChar = stream.ReadNextChar();

                if (!char.IsLetter(currentChar))
                {
                    prevWasLetter = false;
                    continue;
                }

                if (prevWasLetter && char.ToLower(prevChar) == char.ToLower(currentChar))
                {
                    var pairKey = new string(char.ToLower(currentChar), 2);
                    var stat = stats.FirstOrDefault(stat => stat.Char == pairKey);
                    if (stat == null)
                    {
                        stats.Add(new CharStats { Char = pairKey, Count = 1 });
                    }

                    else
                    {
                        IncStatistic(stat);
                    }
                }

                prevChar = currentChar;
                prevWasLetter = true;
            }

            return stats;
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="stats">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(IList<CharStats> stats, CharType charType)
        {
            for (int i = stats.Count - 1; i >= 0; i--)
            {
                bool isVowel = CharTypeHelper.IsVowel(stats[i].Char[0]);

                bool shouldRemove = charType == CharType.Vowel ? isVowel : !isVowel;

                if (shouldRemove)
                {
                    stats.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="stats">Коллекция со статистикой</param>
        private static void PrintStatistic(IEnumerable<CharStats> stats)
        {
            // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
            throw new NotImplementedException();
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="charStats"></param>
        private static void IncStatistic(CharStats charStats)
        {
            charStats.Count++;
        }
    }
}
