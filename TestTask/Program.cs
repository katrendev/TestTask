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
            var singleLetterStats = new List<LetterStats>();
            var doubleLetterStats = new List<LetterStats>();

            using (var inputStream1 = GetInputStream(args[0]))
            {
                 singleLetterStats = FillSingleLetterStats(inputStream1).ToList();
            }

            using (var inputStream2 = GetInputStream(args[0]))
            {
                doubleLetterStats = FillDoubleLetterStats(inputStream2).ToList();
            }


            singleLetterStats = RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            doubleLetterStats = RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            Console.WriteLine("Signle Letter Stats");
            PrintStatistic(singleLetterStats);
            Console.WriteLine("Double Letter Stats");
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


        private static bool IsRussianLetter(char c) => ((c >= 'А') && (c <= 'Я')) || ((c >= 'а') && (c <= 'я'));
        
        private static HashSet<char> consonantsHash = new HashSet<char>
            {
                'б', 'в', 'г', 'д', 'ж', 'з', 'й', 'к', 'л', 'м', 'н', 'п', 'р', 'с', 'т', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ь', 'ъ',
                'Б', 'В', 'Г', 'Д', 'Ж', 'З', 'Й', 'К', 'Л', 'М', 'Н', 'П', 'Р', 'С', 'Т', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Ь', 'Ъ' // продублировано, чтобы лишний раз понижать/повышать регистр у чара
            };

        private static HashSet<char> vowelHash = new HashSet<char>
            {
                'а', 'о', 'и', 'е', 'ё', 'э', 'ы', 'у', 'ю', 'я',
                'А', 'О', 'И', 'Е', 'Ё', 'Э', 'Ы', 'У', 'Ю', 'Я', // продублировано, чтобы лишний раз понижать/повышать регистр у чара
            };

        private static bool IsVowel(char c) => vowelHash.Contains(c);
        private static bool IsConsonants(char c) => consonantsHash.Contains(c);

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
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (stream.IsEof)
                { 
                    continue;
                }

                if (IsRussianLetter(c))
                {
                    if (dict.ContainsKey(c))
                    {
                        var item = dict[c];
                        item.Count++;
                        dict[c] = item;
                    }
                    else
                    {
                        var newItem = new LetterStats { Count = 1, Letter = c.ToString() };
                        dict.Add(c, newItem);
                    }
                }
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
            // по реализованной мною логике последовательность вида ААА - будет считаться за две пары
            var dict = new Dictionary<char, LetterStats>();

            stream.ResetPositionToStart();
            char? prevChar = null;

            while (!stream.IsEof)
            {
                char c = Char.ToUpper(stream.ReadNextChar());
                if (stream.IsEof)
                {
                    continue;
                }

                if (IsRussianLetter(c) && prevChar == c)
                {
                    if (dict.ContainsKey(c))
                    {
                        var item = dict[c];
                        item.Count++;
                        dict[c] = item;
                    }
                    else
                    {
                        var newItem = new LetterStats { Count = 1, Letter = c.ToString() };
                        dict.Add(c, newItem);
                    }
                }

                prevChar = c;
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
        private static List<LetterStats> RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            switch (charType)
            {
                case CharType.Consonants:
                    return letters.Where(l => !IsConsonants(l.Letter.FirstOrDefault())).ToList();
                case CharType.Vowel:
                    return letters.Where(l => !IsVowel(l.Letter.FirstOrDefault())).ToList();
                default:
                    throw new Exception($"unknown {nameof(charType)} value");
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
            {
                Console.WriteLine($"{letter.Letter} - {letter.Count}");
            }
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
