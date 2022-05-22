using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace TestTask
{
    public class Program
    {
        private readonly static IList<char> _cyrillicVowels = new List<char> { 'а', 'э', 'ы', 'у', 'о', 'я', 'е', 'ё', 'ю', 'и', 'A', 'Э', 'Ы', 'У', 'О', 'Я', 'Е', 'Ё', 'Ю', 'И' };

        private readonly static IList<char> _latinVowels = new List<char> { 'a', 'e', 'i', 'o', 'u', 'A', 'E', 'I', 'O', 'U' };

        private static IList<char> _vowels = _cyrillicVowels;

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
            IReadOnlyStream inputStream1 = GetInputStream(args[0]);
            IReadOnlyStream inputStream2 = GetInputStream(args[1]);

            IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
            IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

            singleLetterStats = RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            doubleLetterStats = RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats, "Статистика вхождения каждой буквы:");
            PrintStatistic(doubleLetterStats, "Статистика вхождения парных букв:");

            // TODO - Done : Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
            Console.WriteLine("Нажмите любую клавишу, чтобы завершить выполнение программы");
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

            var dict = new Dictionary<char, LetterStats>();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                // TODO - Done : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
                if (Char.IsLetter(c))
                    ProcessChar(dict, c);
            }

            return dict.Select(kvp => kvp.Value).ToList();
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

            var dict = new Dictionary<char, LetterStats>();
            char prevChar = default;

            while (!stream.IsEof)
            {
                char c = Char.ToLower(stream.ReadNextChar());
                // TODO - Done : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.

                if (Char.IsLetter(c) && prevChar == c)
                {
                    // Намеренно не стал использовать в качестве ключа парные символы, по тупи АА,
                    // т.к. пришлось бы использовать в качестве ключа тип String. Это бы привело к дополнительным аллокациям памяти в куче.
                    // При этом нет ограничения по размеру файла и алоокация строк может случаться очень много.
                    ProcessChar(dict, c);
                }

                prevChar = c;
            }

            return dict.Select(kvp => kvp.Value).ToList();
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static IList<LetterStats> RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            // TODO - Done : Удалить статистику по запрошенному типу букв.
            switch (charType)
            {
                case CharType.Consonants:
                    letters = letters.Where(x => !_vowels.Contains(x.Letter)).ToList();
                    break;
                case CharType.Vowel:
                    letters = letters.Where(x => _vowels.Contains(x.Letter)).ToList();
                    break;
            }

            return letters;
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(IEnumerable<LetterStats> letters, string title = null)
        {
            if (title != null)
                Console.WriteLine(title);
            // TODO - Done : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
            var sorted = letters.OrderBy(ls => ls.Letter);
            foreach (var ls in sorted)
                Console.WriteLine(string.Format("{0} : {1}", ls.Letter, ls.Count));
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(ref LetterStats letterStats)
        {
            letterStats.Count++;
        }

        /// <summary>
        /// Метод безопасно проверяет наличие статистики в словаре или добавляет статистику по определнному ключу
        /// </summary>
        /// <param name="dict">Словарь</param>
        /// <param name="c">Символ</param>
        private static void ProcessChar(Dictionary<char, LetterStats> dict, char c)
        {
            dict.TryGetValue(c, out LetterStats stats);
            if (stats.Letter == default)
                stats.Letter = c;
            IncStatistic(ref stats);
            dict[c] = stats;
        }
    }
}
