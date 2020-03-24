using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TestTask
{
    public class Program
    {
        private static string vowel = "aAeEiIoOuUyY";
        private static string consonants = "bBcCdDfFgGhHjJkKlLmMnNpPqQrRsStTvVwWxXzZ";

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
            IList<LetterStats> singleLetterStats;
            IList<LetterStats> doubleLetterStats;

            try
            {
                using (IReadOnlyStream inputStream1 = GetInputStream(args[0]))
                {
                    singleLetterStats = FillSingleLetterStats(inputStream1);
                }

                using (IReadOnlyStream inputStream2 = GetInputStream(args[1]))
                {
                    doubleLetterStats = FillDoubleLetterStats(inputStream2);
                }

                RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

                PrintStatistic(singleLetterStats);
                PrintStatistic(doubleLetterStats);
            }
            catch (Exception exc)
            {
                Console.WriteLine($"ERROR: {exc.Message}");
            }
            // TODO : Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
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
            var result = new List<LetterStats>();

            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                var c = stream.ReadNextChar();
                if (char.IsLetter(c))
                {
                    var letter = new LetterStats { Letter = c.ToString(), Count = 1 };

                    if (result.Contains(letter))
                        IncStatistic(result[result.IndexOf(letter)]);
                    else
                    {
                        result.Add(letter);
                    }
                }
            }

            return result;
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

            var result = new List<LetterStats>();
            var prevChar = Char.MinValue; //default(char);

            while (!stream.IsEof)
            {
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.
                var c = stream.ReadNextChar();
                if (char.IsLetter(c) && (char.ToUpper(prevChar) == char.ToUpper(c)))
                {
                    var letter = new LetterStats() { Letter = string.Concat(char.ToUpper(prevChar), char.ToUpper(c)), Count = 1 };

                    if (result.Contains(letter))
                    {
                        IncStatistic(result[result.IndexOf(letter)]);
                    }
                    else
                    {
                        result.Add(letter);
                    }
                }
                prevChar = c;
            }

            return result;
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
            // TODO : Удалить статистику по запрошенному типу букв.
            var tempList = letters.ToArray();
            foreach (LetterStats letter in tempList)
            {
                switch (charType)
                {
                    case CharType.Consonants:
                        {
                            if (consonants.Contains(letter.Letter[0]))
                                letters.Remove(letter);
                            break;
                        }

                    case CharType.Vowel:
                        {
                            if (vowel.Contains(letter.Letter[0]))
                                letters.Remove(letter);
                            break;
                        }
                }
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
            // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
            var ordered = letters.OrderBy<LetterStats, string>(s => s.Letter);

            foreach (LetterStats stats in ordered)
            {
                Console.WriteLine($"Letter: {stats.Letter} Count: {stats.Count}");
            }

            var summ = letters.Sum<LetterStats>(s => s.Count);
            Console.WriteLine($"Summ: {summ}");
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
        }
    }
}
