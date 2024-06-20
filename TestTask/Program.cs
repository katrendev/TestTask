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
            IList<LetterStats> singleLetterStats = null;
            using (IReadOnlyStream inputStream = GetInputStream(args[0]))
            {
                singleLetterStats = FillSingleLetterStats(inputStream);
            }

            IList<LetterStats> doubleLetterStats = null;
            using (IReadOnlyStream inputStream = GetInputStream(args[1]))
            {
                doubleLetterStats = FillDoubleLetterStats(inputStream);
            }

            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            Console.WriteLine();
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
            stream.ResetPositionToStart();
            Dictionary<string, LetterStats> letterCount = new Dictionary<string, LetterStats>();

            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (char.IsLetter(c))
                {
                    string stringC = c.ToString();
                    if (letterCount.ContainsKey(stringC))
                    {
                        letterCount[stringC].IncStatistic();
                    }
                    else
                    {
                        letterCount[stringC] = new LetterStats(stringC);
                    }
                }
            }

            return letterCount.Values.ToList();
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
            Dictionary<string, LetterStats> doubleLetterCount = new Dictionary<string, LetterStats>();

            char c = stream.ReadNextChar();
            while (!stream.IsEof)
            {
                char nextChar = stream.ReadNextChar();
                if (char.IsLetter(c) && char.IsLetter(nextChar) && (char.ToUpper(c) == char.ToUpper(nextChar)))
                {
                    string doubleLetter = (char.ToUpper(c).ToString() + char.ToUpper(nextChar).ToString());
                    if (doubleLetterCount.ContainsKey(doubleLetter))
                    {
                        doubleLetterCount[doubleLetter].IncStatistic();
                    }
                    else
                    {
                        doubleLetterCount[doubleLetter] = new LetterStats(doubleLetter);
                    }
                }
                c = nextChar;
            }
            return doubleLetterCount.Values.ToList();
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
                if ((charType == CharType.Consonants && IsVowel(letters[i].Letter[0])) ||
                    (charType == CharType.Vowel && !IsVowel(letters[i].Letter[0])))
                {
                    letters.RemoveAt(i);
                }
            }
        }

        private static bool IsVowel(char letter)
        {
            if (!char.IsLetter(letter))
                return false;

            string vowels = "АЕЁИОУЫЭЮЯаеёиоуыэюя";
            return vowels.Contains(letter);
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
            foreach (var stat in stats.OrderBy(s => s.Letter))
            {
                Console.WriteLine(stat);
            }
            int total = stats.Sum(s => s.Count);
            Console.WriteLine($"ИТОГО: {total}");
        }
    }
}
