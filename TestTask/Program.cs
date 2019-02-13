using System;
using System.Collections.Generic;
using System.Text;

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
            args = new string[] { $"{AppDomain.CurrentDomain.BaseDirectory}1.txt", $"{AppDomain.CurrentDomain.BaseDirectory}2.txt" };
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
            Dictionary<char, LetterStats> _letters = new Dictionary<char, LetterStats>();
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (!IsLetter(c))
                {
                    continue;
                }
                if (!_letters.ContainsKey(c))
                {
                    _letters.Add(c, new LetterStats() { Count = 1, Letter = c.ToString() });
                }
                else
                {
                    IncStatistic(_letters[c]);
                }
            }

            LetterStats[] result = new LetterStats[_letters.Values.Count];
            _letters.Values.CopyTo(result, 0);

            return result;
        }

        /// <summary>
        /// Ф-ция для проверки является ли символ буквой
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static bool IsLetter(char c)
        {
            return (c >= 65 && c <= 90) || (c >= 97 && c <= 122);
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
            char buf = ' ';
            bool isFirst = true;
            Dictionary<string, LetterStats> _doubleLetters = new Dictionary<string, LetterStats>();
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (!IsLetter(c))
                {
                    continue;
                }
                if (isFirst)
                {
                    buf = c;
                    isFirst = false;
                }
                else
                {
                    if (buf.ToString().ToLower() == c.ToString().ToLower())
                    {
                        string key = new string(new char[] { buf, c }).ToLower();
                        if (!_doubleLetters.ContainsKey(key))
                        {
                            _doubleLetters.Add(key, new LetterStats() { Letter = key, Count = 1 });
                        }
                        else
                        {
                            IncStatistic(_doubleLetters[key]);
                        }
                    }
                    else
                    {
                        buf = '1';
                    }
                    isFirst = true;
                }
            }
            LetterStats[] result = new LetterStats[_doubleLetters.Values.Count];
            _doubleLetters.Values.CopyTo(result, 0);

            return result;
        }

        private static HashSet<int> GetVowelLettersHashSet()
        {
            return new HashSet<int>(new int[] { 65, 69, 73, 79, 85, 89, 97, 101, 105, 111, 117, 121 });
        }

        private static HashSet<int> GetConsonantLettersHashset()
        {
            return new HashSet<int>(new int[] { 66, 67, 68, 70, 71, 72, 74, 75, 76, 77, 78, 80, 81, 82, 83, 84, 86, 87, 88, 90,
                98, 99, 100, 102, 103, 104, 106, 107, 108, 109, 110, 112, 113, 114, 115, 116, 118, 119, 120, 122 });        
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
            HashSet<int> lettersHashSet = null;
            switch (charType)
            {
                case CharType.Consonants:
                    lettersHashSet = GetConsonantLettersHashset();
                    break;
                case CharType.Vowel:
                    lettersHashSet = GetVowelLettersHashSet();
                    break;
                default:
                    throw new ArgumentException("Некорректный тип буквы", "charType");
            }
            List<LetterStats> listLetters = new List<LetterStats>();         
            for(int i = 0; i < letters.Count; i++)
            {
                int ci = (int)letters[i].Letter[0];
                if (!lettersHashSet.Contains(ci))
                {
                    listLetters.Add(letters[i]);
                }
            }
            letters = listLetters;
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
            List<LetterStats> listLetters = new List<LetterStats>(letters);
            listLetters.Sort((x,y) => x.Letter.CompareTo(y.Letter));
            StringBuilder sb = new StringBuilder();
            foreach (LetterStats let in letters)
            {
                sb.Append($"{let.Letter} : {let.Count} \r\n");
            }
            sb.Append($"ИТОГО: {listLetters.Count}\r\n");

            Console.Write(sb.ToString());
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
