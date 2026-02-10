using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    public class Program
    {
        private const string _vowelLetters = "aeiou";
        private static List<char> vowelLettersList;
        private static Dictionary<CharType, Func<LetterStats, bool>> funcByCharType;

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
            Init();

            using (var inputStream = GetInputStream(args[0])) 
            {
                IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream);
                PrintStatistic(singleLetterStats);
                RemoveCharStatsByType(ref singleLetterStats, CharType.Vowel);
                PrintStatistic(singleLetterStats);
            }
            using (var inputStream = GetInputStream(args[1]))
            {
                IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream);
                PrintStatistic(doubleLetterStats);
                RemoveCharStatsByType(ref doubleLetterStats, CharType.Consonants);
                PrintStatistic(doubleLetterStats);
            }
            Console.ReadKey();
        }

        private static void Init()
        {
            InitCharTypes();
            InitVowelLetters();
        }

        private static void InitCharTypes()
        {
            funcByCharType = new Dictionary<CharType, Func<LetterStats, bool>>();
            funcByCharType.Add(CharType.Vowel, x => IsVowel(x.Letter));
            funcByCharType.Add(CharType.Consonants, x => !IsVowel(x.Letter));
        }

        private static void InitVowelLetters()
        {
            vowelLettersList = new List<char>();
            var vowelLettersLowerList = _vowelLetters.ToLower().ToList();
            var vowelLettersUpperList = _vowelLetters.ToUpper().ToList();
            vowelLettersList.AddRange(vowelLettersLowerList);
            vowelLettersList.AddRange(vowelLettersUpperList);
        }

        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        private static ReadOnlyStream GetInputStream(string fileFullPath)
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
            Dictionary<string,int> stats = new Dictionary<string, int>();
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                var ch = stream.ReadNextChar().ToString();
                if (stats.ContainsKey(ch))
                {
                    stats[ch]++;
                }
                else
                {
                    stats.Add(ch, 1);
                }
                
            }
            var letterStats = Convert(stats);

            return letterStats;
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
            Dictionary<string, int> stats = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            stream.ResetPositionToStart();
            var leftC = stream.ReadNextChar().ToString();
            while (!stream.IsEof)
            {
                var rightC = stream.ReadNextChar().ToString();

                if (string.Equals(rightC, leftC, StringComparison.CurrentCultureIgnoreCase))
                {
                    var strPair = leftC + rightC;
                    if (stats.ContainsKey(strPair))
                    {
                        stats[strPair]++;
                    }
                    else
                    {
                        stats.Add(strPair, 1);
                    }

                }
                leftC = rightC;
            }
            var letterStats = Convert(stats);

            return letterStats;
        }

        private static List<LetterStats> Convert(Dictionary<string, int> stats)
        {
            List<LetterStats> letterStats = new List<LetterStats>();
            foreach (var stat in stats)
            {
                var letterStat = new LetterStats();
                letterStat.Letter = stat.Key;
                letterStat.Count = stat.Value;
                letterStats.Add(letterStat);
            }
            return letterStats;
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(ref IList<LetterStats> letters, CharType charType)
        {
            letters = letters.Where(x => funcByCharType[charType].Invoke(x)).ToList();
        }

        private static bool IsVowel(string str)
        {
            foreach (var ch in str)
            {
                if (!vowelLettersList.Contains(ch))
                {
                    return false;
                }
            }
            return true;
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
            foreach (var letter in letters.OrderBy(x => x.Letter))
            {
                Console.WriteLine(string.Format("{0} : {1}", letter.Letter, letter.Count));
            }
            Console.WriteLine(string.Format("Итого: {0}",letters.Count()));
        }
    }
}
