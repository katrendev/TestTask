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
        /// <param name="args">Первый параметр - путь до первого файла.
        /// Второй параметр - путь до второго файла.</param>
        static void Main(string[] args)
        {
            if (args == null || args.Length < 2)
            {
                Console.WriteLine("Нужно передать 2 параметра: <путь_к_файлу_1> <путь_к_файлу_2>");
                Console.WriteLine("Пример: TestTask.exe \"C:\\temp\\1.txt\" \"C:\\temp\\2.txt\"");
                Console.ReadKey();
                return;
            }

            IReadOnlyStream inputStream1 = GetInputStream(args[0]);
            IReadOnlyStream inputStream2 = GetInputStream(args[1]);

            IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
            IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

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
                char c;

                try { c = stream.ReadNextChar(); }
                catch { break; }

                if (!char.IsLetter(c))
                    continue;

                string key = c.ToString();

                if (!dict.TryGetValue(key, out var stat))
                {
                    stat = new LetterStats { Letter = key };
                    dict[key] = stat;
                }

                IncStatistic(stat);
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

            stream.ResetPositionToStart();

            char? prev = null;

            while (!stream.IsEof)
            {
                char c;

                try { c = stream.ReadNextChar(); }
                catch { break; }

                if (!char.IsLetter(c))
                {
                    prev = null;
                    continue;
                }

                if (prev.HasValue &&
                    char.ToUpperInvariant(prev.Value) == char.ToUpperInvariant(c))
                {
                    char up = char.ToUpperInvariant(c);
                    string key = new string(new[] { up, up });

                    if (!dict.TryGetValue(key, out var stat))
                    {
                        stat = new LetterStats { Letter = key };
                        dict[key] = stat;
                    }

                    IncStatistic(stat);
                }

                prev = c;
            }

            return dict.Values.ToList();
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
                var item = letters[i];

                bool match = item.Letter.All(c =>
                    charType == CharType.Vowel ? IsVowel(c) : !IsVowel(c));

                if (match)
                    letters.RemoveAt(i);
            }
        }

        private static bool IsVowel(char c)
        {
            c = char.ToUpperInvariant(c);
            return "AEIOUYАЕЁИОУЫЭЮЯ".Contains(c);
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
            if (letters == null)
            {
                Console.WriteLine("ИТОГО : 0");
                return;
            }

            var ordered = letters
                .Where(x => x != null && !string.IsNullOrEmpty(x.Letter))
                .OrderBy(x => x.Letter, StringComparer.CurrentCulture)
                .ToList();

            int total = 0;

            foreach (var item in ordered)
            {
                Console.WriteLine($"{item.Letter} : {item.Count}");
                total += item.Count;
            }

            Console.WriteLine($"ИТОГО : {total}");
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
