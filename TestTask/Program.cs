using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    public class Program
    {
        private static readonly string Vowels = "aeiouAEIOUаеёиоуыэюяАЕЁИОУЫЭЮЯ";
        
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
            void readFile(string path, Func<IReadOnlyStream, IList<LetterStats>> fillstats, CharType charType)
            {
                using (var inputStream = GetInputStream(path))
                {
                    if (inputStream != null)
                    {
                        var letterStats = fillstats(inputStream);
                        RemoveCharStatsByType(letterStats, charType);
                        PrintStatistic(letterStats.OrderBy(st => st.Letter));
                    }
                }
            };

            if (args.Length > 0)
                readFile(args[0], FillSingleLetterStats, CharType.Consonants);
            if (args.Length > 1)
                readFile(args[1], FillDoubleLetterStats, CharType.Vowel);

            Console.ReadKey();
        }

        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        private static IReadOnlyStream GetInputStream(string fileFullPath)
        {
            try
            {
                return new ReadOnlyStream(fileFullPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
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
            var stats = new List<LetterStats>();
            while (!stream.IsEof)
            {
                var ch = stream.ReadNextChar();
                if (char.IsLetter(ch))
                    IncStatistic(stats, ch.ToString());
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
        private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            stream.ResetPositionToStart();
            var stats = new List<LetterStats>();
            if (stream.IsEof)
                return stats;
            var chars = new char[2];
            chars[0] = stream.ReadNextChar();

            while (!stream.IsEof)
            {
                chars[1] = stream.ReadNextChar();
                if (char.IsLetter(chars[0]) && char.IsLetter(chars[1]))
                {
                    var str = new string(chars).ToLower();
                    IncStatistic(stats, str);
                }
                chars[0] = chars[1];
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
        private static void RemoveCharStatsByType(IEnumerable<LetterStats> stats, CharType charType)
        {
            bool IsVowel(char c) => Vowels.Contains(c);
            switch (charType)
            {
                case CharType.Consonants:
                    stats = stats.Where(st => st.Letter.All(c => !IsVowel(c)));
                    break;
                case CharType.Vowel:
                    stats = stats.Where(st => st.Letter.All(c => IsVowel(c)));
                    break;
            }
            
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(IEnumerable<LetterStats> stats)
        {
            Console.WriteLine();
            var sum = 0;
            foreach (var st in stats)
            {
                Console.WriteLine($"{st.Letter} : {st.Count}");
                sum += st.Count;
            }
            Console.WriteLine($"ИТОГО : {sum}");
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной строке.
        /// </summary>
        /// <param name="stats">Коллекция со статистикой</param>
        /// <param name="letter">Добавляемая строка</param>
        private static void IncStatistic(IList<LetterStats> stats, string letter)
        {
            var ls = stats.FirstOrDefault(st => st.Letter == letter);
            if (ls != null)
                ls.Count++;
            else
                stats.Add(new LetterStats(letter));
        }
    }
}
