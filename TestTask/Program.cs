using System;
using System.Collections.Generic;
using System.IO;
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
        /// <param name="args">Первый параметр - путь до первого файла.
        /// Второй параметр - путь до второго файла.</param>
        static void Main(string[] args)
        {

            //для работы с файлами, чтобы вам не пришлось создавать)))
            string file1 = args.Length > 0
                ? args[0]
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", "file1.txt");
            string file2 = args.Length > 1
                ? args[1]
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", "file2.txt");

            Console.WriteLine($"File1: {file1}");
            Console.WriteLine($"File2: {file2}");

            IReadOnlyStream input1 = null;
            IReadOnlyStream input2 = null;
            try
            {
                input1 = GetInputStream(file1);
                input2 = GetInputStream(file2);

                var singleLetterStats = FillSingleLetterStats(input1);
                var doubleLetterStats = FillDoubleLetterStats(input2);

                RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

                PrintStatistic(singleLetterStats);
                PrintStatistic(doubleLetterStats);
            }
            finally
            {
                input1?.Dispose();
                input2?.Dispose();
            }

            Console.WriteLine("Нажмите любую клавишу для выхода...");
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
            var dict = new Dictionary<char, LetterStats>();
            stream.ResetPositionToStart();
            while(!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (!char.IsLetter(c)) continue;

                //регистрозависимо
                if (!dict.ContainsKey(c))
                    dict[c] = new LetterStats { Letter = c.ToString(), Count = 0 };
                dict[c].Count++;
            }
            return dict.Values.OrderBy(s => s.Letter, StringComparer.Ordinal).ToList();
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
            var dict = new Dictionary<string, LetterStats>(StringComparer.OrdinalIgnoreCase);
            stream.ResetPositionToStart();
            char? prev = null;
            while (!stream.IsEof)
            {
                char c = char.ToUpperInvariant(stream.ReadNextChar());
                if (!char.IsLetter(c))
                {
                    prev = null;
                    continue;
                }

                if (prev.HasValue && prev.Value == c)
                {
                    string pair = new string(new[] { c, c });
                    if (!dict.ContainsKey(pair))
                        dict[pair] = new LetterStats { Letter = pair, Count = 0 };
                    dict[pair].Count++;
                }
                prev = c;
            }
            return dict.Values.OrderBy(ls => ls.Letter, StringComparer.OrdinalIgnoreCase).ToList();
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
            bool IsVowel(char x) =>
                "AEIOUYАЕЁИОУЫЭЮЯ".IndexOf(char.ToUpperInvariant(x)) >= 0;

            for (int i = letters.Count - 1; i >= 0; i--)
            {
                string s = letters[i].Letter;
                bool allVowels = s.All(IsVowel);

                if ((charType == CharType.Vowel && allVowels) ||
                    (charType == CharType.Consonants && !allVowels))
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
            int total = 0;
            foreach (var ls in letters)
            {
                Console.WriteLine($"{ls.Letter} : {ls.Count}");
                total += ls.Count;
            }
            Console.WriteLine($"ИТОГО: {total}");
            Console.WriteLine();
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
