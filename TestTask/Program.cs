using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
            try
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("ОШИБКА: Неверное количество аргументов. Требуется указать пути к двум файлам.");
                    return;
                }
                // Обработка файлов никак не связана друг с другом, так что можно делать одновременно
                Task.WaitAll(
                    ProcessFile(args[0], FillSingleLetterStats, CharType.Vowel),
                    ProcessFile(args[1], FillDoubleLetterStats, CharType.Consonants)
                );
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException is FileNotFoundException)
                {
                    // Эта ошибка понятна и полезна для пользователя,
                    // поэтому ообрабатываем ее отдельно и показываем короткое сообщение.
                    // Остальные исключение - это внутренние ошибки, которые ничего не скажут пользователю
                    // поэтому пусть для них выводится стандартный стек-трейс для отправки его разработчику
                    Console.WriteLine(ex.InnerException.Message);
                }
                else throw;
            }
            finally
            {
                // Дождаться нажатия клавиши, прежде чем завершать выполнение программы.
                Console.WriteLine("Для завершения, нажмите любую клавишу...");
                Console.ReadKey(true);
            }
        }

        private static readonly object _consoleLocker = new object();

        private static Task ProcessFile(string filePath, Func<IReadOnlyStream, IList<LetterStats>> calcStats, CharType charType)
        {
            return Task.Run(() =>
            {
                var fileFullPath = Path.GetFullPath(filePath);
                Console.WriteLine($"Обработка {fileFullPath} ...");
                using (var stream = GetInputStream(fileFullPath))
                { 
                    var stats = calcStats(stream);
                    RemoveCharStatsByType(stats, charType);
                    lock (_consoleLocker)
                    {
                        Console.WriteLine($"Статистика для {fileFullPath}");
                        PrintStatistic(stats);
                    }
                }
            });
        }

        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        private static IReadOnlyStream GetInputStream(string fileFullPath)
        {
            return ReadOnlyStream.ForFile(fileFullPath);
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        public static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            stream.ResetPositionToStart();

            var dict = new Dictionary<char, LetterStats>();
            while (!stream.IsEof)
            {
                var ch = stream.ReadNextChar();
                if (!char.IsLetter(ch))
                {
                    continue;
                }
                if (!dict.TryGetValue(ch, out var stat))
                {
                    stat = new LetterStats { Letter = ch.ToString() };
                }
                // заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
                // NOTE: оптимальнее было бы просто собирать статистику в Dictionary<char, int>
                // тогда можно избежать копирования структур и передачи во внешнюю функцию-счетчик
                // А финальный список структур собрать уже в последний момент.
                IncStatistic(ref stat);
                dict[ch] = stat;
            }

            return dict.Values.OrderBy(s => s.Letter).ToList();
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        public static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            stream.ResetPositionToStart();

            char? prevCh = null;
            var dict = new Dictionary<char, LetterStats>();
            while (!stream.IsEof)
            {
                var ch = char.ToLower(stream.ReadNextChar());
                if (!char.IsLetter(ch))
                {
                    prevCh = null;
                    continue;
                }
                if (ch != prevCh)
                { 
                    prevCh = ch;
                    continue;
                }
                if (!dict.TryGetValue(ch, out var stat))
                {
                    stat = new LetterStats { Letter = $"{ch}{ch}" };
                }
                // заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.
                // NOTE: оптимальнее было бы просто собирать статистику в Dictionary<char, int>
                // тогда можно избежать копирования структур и передачи во внешнюю функцию-счетчик
                // А финальный список структур собрать уже в последний момент.
                IncStatistic(ref stat);
                dict[ch] = stat;
                prevCh = null;
            }

            return dict.Values.OrderBy(s => s.Letter).ToList();
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        public static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            int index = 0;
            // Удалить статистику по запрошенному типу букв.
            while (index < letters.Count)
            {
                if (letters[index].CharType == charType)
                {
                    letters.RemoveAt(index);
                    continue;
                }
                index++;
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
            // Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
            // Собранная статистика уже отсортирована в процессе сбора, см. методы Fill*Stats()
            int total = 0;
            foreach (var stat in letters)
            { 
                Console.WriteLine($"{stat.Letter} : {stat.Count}");
                total += stat.Count;
            }
            Console.WriteLine($"ИТОГО: {total}");
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
}
