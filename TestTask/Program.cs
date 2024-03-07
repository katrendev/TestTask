using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestTask
{
    public class Program
    {
        const char DEFAULT_CHAR = default;
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
            using (IReadOnlyStream inputStream1 = GetInputStream(args[0]), inputStream2 = GetInputStream(args[1]))
            {
                IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
                IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

                singleLetterStats = singleLetterStats.Where(x => !x.Letter.IsSpecial()).ToList();
                doubleLetterStats = doubleLetterStats.Where(x => !x.Letter.IsSpecial()).ToList();

                RemoveCharStatsByType(ref singleLetterStats, CharType.Vowel);
                RemoveCharStatsByType(ref doubleLetterStats, CharType.Consonants);

                PrintStatistic(singleLetterStats);
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
        /// Функция добавления буквы в статистику.
        /// В случае отсутствия статистики по добовляемой букве создает и добавляет в коллекцию новый элемент.
        /// Увеличивает счетчик вхождения для соответсвующей буквы/пары букв в статистике.
        /// </summary>
        /// <param name="array">Массив статистик вхождения буквы/пары букв</param>
        /// <param name="letter">Добавляемая буква для учета в статистике</param>
        private static void AddLetterToStats(LetterStats[] array, char letter)
        {
            var index = Array.FindIndex(array, x => x.Letter == DEFAULT_CHAR || x.Letter == letter);

            if (array[index].Letter == DEFAULT_CHAR)
            {
                array[index] = new LetterStats
                {
                    Count = 0,
                    Letter = letter,
                };
            }

            IncStatistic(ref array[index]);
        }
        /// <summary>
        /// Обрезает массив статистик отбрассивая неиспользуемые незаполненные LetterStats
        /// </summary>
        /// <param name="array">Массив для обрезания</param>
        /// <returns>Обрезанный массив</returns>
        private static LetterStats[] TrimArrayStats(LetterStats[] array)
        {
            var index = Array.FindIndex(array, x => x.Letter == DEFAULT_CHAR);
            Array.Resize(ref array, index);
            return array;
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
            LetterStats[] stats = new LetterStats[Char.MaxValue];

            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (!Char.IsLetter(c))
                {
                    continue;
                }
                AddLetterToStats(stats, c);
            }
            return TrimArrayStats(stats);
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
            LetterStats[] stats = new LetterStats[Char.MaxValue];
            char prev = DEFAULT_CHAR;

            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();

                if (!Char.IsLetter(c))
                {
                    prev = c;
                    continue;
                }

                var letter = char.ToLower(c);
                if (letter == prev)
                {
                    AddLetterToStats(stats, letter);
                }
                prev = letter;
            }

            return TrimArrayStats(stats);
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
            /* Не понял формулировку "Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.":
            * Мы должны избавиться от других букв(ь, ъ) в этой функции?
            * Или
            * Ф-ция принимает статистику в которой нет других букв?
            * (Но когда мы от них избавились? Ранее не было условия для статистики "содержащие в себе только гласные или согласные буквы" !)
            */
            switch (charType)
            {
                case CharType.Consonants:
                    letters = letters.Where(x => !x.Letter.IsConsonants()).ToList();
                    break;
                case CharType.Vowel:
                    letters = letters.Where(x => !x.Letter.IsVowel()).ToList();
                    break;
                default:
                    throw new ArgumentException($"Не поддерживаемый аргумент: {charType}");
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
            var sum = 0;
            foreach (var item in letters.OrderBy(x => char.ToLower(x.Letter)).ThenBy(x => x.Letter))
            {
                Console.WriteLine($"{item.Letter} : {item.Count}");
                sum += item.Count;
            }
            Console.WriteLine("ИТОГО " + sum);
        }
        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(ref LetterStats letterStats)
        {
            letterStats.Count++;
        }
    }
    /// <summary>
    /// Класс расширение Сhar
    /// </summary>
    public static class СharExtensions
    {
        /// <summary>
        /// Коллекция глассных букв
        /// </summary>
        // Добавление поддержки новых языков в таком варианте исполнения замедляет работу
        private static HashSet<char> Vowels = new HashSet<char>()
        {
            // Русский
            'а', 'о', 'ы', 'э', 'у', 'я', 'ё', 'и', 'е', 'ю',
            // Английский
            'a', 'o', 'e', 'i', 'u', 'y',
        };
        /// <summary>
        /// Коллекция букв не являющихся ни гласными, ни согласными
        /// </summary>
        private static HashSet<char> Specials = new HashSet<char>()
        {
            'ь', 'ъ',
        };
        /// <summary>
        /// Показывает относится ли указанный символ к категории гласных букв
        /// </summary>
        /// <param name="c">Проверяемый символ</param>
        /// <returns>значение true, если c является гласной буквой; false - в противном случае</returns>
        public static bool IsVowel(this char c)
        {
            return Vowels.Contains(char.ToLower(c));
        }
        /// <summary>
        /// Показывает относится ли указанный символ к категории букв не являющихся ни гласными, ни согласными
        /// </summary>
        /// <param name="c">Проверяемый символ</param>
        /// <returns>значение true, если c является буквой не являющейся ни гласной, ни согласной; false - в противном случае</returns>
        public static bool IsSpecial(this char c)
        {
            return Specials.Contains(char.ToLower(c));
        }
        /// <summary>
        /// Показывает относится ли указанный символ к категории согласных букв
        /// </summary>
        /// <param name="c">Проверяемый символ</param>
        /// <returns>значение true, если c является согласной буквой; false - в противном случае</returns>
        public static bool IsConsonants(this char c)
        {
            return !IsSpecial(c)
                && !IsVowel(c);
        }
    }
}
