using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    public class Program
    {
        private static readonly HashSet<char> Vowels = new HashSet<char>(
            "аеёиоуыэюяАЕЁИОУЫЭЮЯaeiouAEIOU"
        );

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
            if (args == null || args.Length < 2)
            {
                Console.WriteLine("Ошибка: Недостаточно аргументов или файлы не найдены");

                Console.WriteLine("\nНажмите любую клавишу для закрытия консоли...");
                Console.ReadKey();
                return;
            }

            try
            {
                // Используем блоки using для автоматического закрытия файлов
                using (IReadOnlyStream inputStream1 = GetInputStream(args[0]))
                using (IReadOnlyStream inputStream2 = GetInputStream(args[1]))
                {
                    IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
                    IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

                    RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                    RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

                    PrintStatistic(singleLetterStats);
                    PrintStatistic(doubleLetterStats);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка при обработке файлов: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу для закрытия консоли...");
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
            var statsMap = new Dictionary<string, int>();
            stream.ResetPositionToStart();

            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (char.IsLetter(c))
                {
                    string key = c.ToString();
                    if (!statsMap.ContainsKey(key)) statsMap[key] = 0;
                    statsMap[key]++;
                }
            }

            return statsMap.Select(kvp => new LetterStats { Letter = kvp.Key, Count = kvp.Value }).ToList();
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
            var statsMap = new Dictionary<string, int>();
            stream.ResetPositionToStart();

            char? prevChar = null;

            while (!stream.IsEof)
            {
                char current = stream.ReadNextChar();

                if (char.IsLetter(current))
                {
                    if (prevChar.HasValue && char.ToLower(prevChar.Value) == char.ToLower(current))
                    {
                        string key = (current.ToString() + current.ToString()).ToLower();
                        if (!statsMap.ContainsKey(key)) statsMap[key] = 0;
                        statsMap[key]++;

                        // После нахождения пары сбрасываем, чтобы "aaa" считалось как одна пара "aa" 
                        prevChar = null;
                        continue;
                    }
                    prevChar = current;
                }
                else
                {
                    prevChar = null;
                }
            }

            return statsMap.Select(kvp => new LetterStats { Letter = kvp.Key, Count = kvp.Value }).ToList();
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
            for (int i = letters.Count - 1; i >= 0; i--)
            {
                bool isVowel = Vowels.Contains(letters[i].Letter.ToLower()[0]);

                if (charType == CharType.Vowel && isVowel)
                {
                    letters.RemoveAt(i);
                }
                else if (charType == CharType.Consonants && !isVowel)
                {
                    letters.RemoveAt(i);
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
            var sorted = letters.OrderBy(l => l.Letter);
            int total = 0;

            foreach (var stat in sorted)
            {
                Console.WriteLine($"{stat.Letter} : {stat.Count}");
                total += stat.Count;
            }

            Console.WriteLine($"ИТОГО : {total}");
        }
    }
}
