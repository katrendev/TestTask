using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    public class Program
    {
        private static HashSet<string> Vowels = new HashSet<string>
        {  "а", "о", "у", "е", "ы", "я", "и", "ё", "ю", "a", "e", "i", "o", "u", "y" };

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
            using (IReadOnlyStream inputStream1 = GetInputStream(args[0]))
            {
                IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
                RemoveCharStatsByType(ref singleLetterStats, CharType.Vowel);
                PrintStatistic(singleLetterStats);
            }

            using (IReadOnlyStream inputStream2 = GetInputStream(args[1]))
            {
                IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);
                RemoveCharStatsByType(ref doubleLetterStats, CharType.Consonants);
                PrintStatistic(doubleLetterStats);
            }

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
            var letterStat = new Dictionary<string, LetterStats>();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (!Char.IsLetter(c))
                    continue;

                string letter = c.ToString();
                if (letterStat.ContainsKey(letter))
                {
                    letterStat[letter] = IncStatistic(letterStat[letter]);
                }
                else
                {
                    CharType type = GetLetterType(letter);
                    letterStat[letter] = new LetterStats
                    {
                        Count = 1,
                        Type = type,
                        Letter = letter
                    };
                }
            }

            return letterStat.Values.ToList();
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
            var letterStat = new Dictionary<string, LetterStats>();
            string prev = string.Empty;
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (!Char.IsLetter(c))
                    continue;

                string letter = c.ToString().ToLower();
                if (String.Equals(letter, prev))
                {
                    if (letterStat.ContainsKey(letter))
                    {
                        letterStat[letter] = IncStatistic(letterStat[letter]);
                    }
                    else
                    {
                        var type = GetLetterType(letter);
                        letterStat[letter] = new LetterStats
                        {
                            Count = 1,
                            Type = type,
                            Letter = letter
                        };
                    }
                }
                prev = letter;
            }
            return letterStat.Values.ToList();
        }

        private static CharType GetLetterType(string letter) => IsVowel(letter) ? CharType.Vowel : CharType.Consonants;

        private static bool IsVowel(string letter) => Vowels.Contains(letter.ToLower());

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(ref IList<LetterStats> letters, CharType charType)
        {
            letters = letters.Where(x => x.Type != charType).ToList();
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
            int count = 0;
            letters = letters.OrderBy(x => x.Letter).ToList();
            foreach (LetterStats item in letters)
            {
                Console.WriteLine($"{item.Letter} : {item.Count}");
                count += item.Count;
            }
            Console.WriteLine($"ИТОГО: {count}");
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static LetterStats IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
            return letterStats;
        }
    }
}
