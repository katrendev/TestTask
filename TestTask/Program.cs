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
#if DEBUG
            args = new string[]
            {
                "file1.txt",
                "file2.txt"
            };
#endif
            using (IReadOnlyStream inputStream1 = GetInputStream(args[0]))
            using (IReadOnlyStream inputStream2 = GetInputStream(args[1]))
            {
                try
                {
                    IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
                    IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

                    RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                    RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

                    Console.WriteLine("Single Letter Stats");
                    PrintStatistic(singleLetterStats);
                    Console.WriteLine("Double Letter Stats");
                    PrintStatistic(doubleLetterStats);
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                }
                finally
                {
                    Console.ReadKey();
                }
            }
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
            var stats = new List<LetterStats>();

            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                var stat = stats.Find(s => s.Letter == c.ToString());
                if (stat.Letter != null)
                {
                    IncStatistic(ref stat);
                    continue;
                }
                stats.Add(new LetterStats { Count = 1, Letter = c.ToString() });
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
            var stats = new List<LetterStats>();
            string prevC;
            string nextC = stream.ReadNextChar().ToString();

            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                prevC = nextC;
                nextC = stream.ReadNextChar().ToString();
                if (prevC.ToLower() != nextC.ToLower())
                    continue;
                var pair = prevC + nextC;
                var stat = stats.Find(s => s.Letter.ToLower() == pair.ToLower());
                if (stat.Letter != null)
                {
                    stats.Remove(stat);
                    IncStatistic(ref stat);
                    stats.Add(stat);
                    continue;
                }
                stats.Add(new LetterStats { Count = 1, Letter = pair });
            }

            return stats;
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
            switch (charType)
            {
                case CharType.Consonants:
                    var consonants = new string[] { "а", "у", "о", "ы", "и", "э", "я", "ю", "ё", "е" };
                    RemoveCharsStatsByPattern(letters, consonants);
                    break;
                case CharType.Vowel:
                    var vowel = new string[] { "б", "в", "г", "д", "ж", "з", "й", "к", "л", "м", "н", "п", "р", "с", "т", "ф", "х", "ц", "ч", "ш", "щ" };
                    RemoveCharsStatsByPattern(letters, vowel);
                    break;
            }
        }

        private static void RemoveCharsStatsByPattern(IList<LetterStats> letters, string[] pattern)
        {
            foreach (var v in pattern)
            {
                var toRemove = letters.Where(l => l.Letter.ToLower().Contains(v));
                while (toRemove.Count() > 0)
                    letters.Remove(toRemove.ElementAt(0));
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
            foreach (var letter in letters.OrderBy(l => l.Letter))
                Console.WriteLine($"{letter.Letter}\t:\t{letter.Count}");
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(ref LetterStats letterStats) => letterStats.Count++;
    }
}
