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
            if (args.Length < 2)            
                throw new ArgumentException("Needs 2 arguments to work.");

            using (var inputStream1 = GetInputStream(args[0]))
            {
                var singleLetterStats = FillSingleLetterStats(inputStream1);
                RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                PrintStatistic(singleLetterStats);
            }

            using (var inputStream2 = GetInputStream(args[1]))
            {
                var doubleLetterStats = FillDoubleLetterStats(inputStream2);
                RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);
                PrintStatistic(doubleLetterStats);
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
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
            var letterStats = new Dictionary<string, LetterStats>();

            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                var currentChar = stream.ReadNextChar();

                if (!char.IsLetter(currentChar))
                    continue;

                var latter = currentChar.ToString();

                if (!letterStats.TryGetValue(latter, out var letterStat))
                {
                    letterStat = new LetterStats { Letter = latter, Count = 0 };
                    letterStats.Add(latter, letterStat);
                }

                IncStatistic(ref letterStat);
                letterStats[latter] = letterStat;
            }

            return letterStats.Values.ToList();
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
            var letterStats = new Dictionary<string, LetterStats>();
            var previousChar = '\0';

            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                var currentChar = char.ToUpper(stream.ReadNextChar());

                if (char.IsLetter(previousChar) && char.IsLetter(currentChar))
                {
                    if (currentChar == previousChar)
                    {
                        var latter = $"{previousChar}{currentChar}";

                        if (!letterStats.TryGetValue(latter, out var letterStat))
                        {
                            letterStat = new LetterStats { Letter = latter, Count = 0 };
                            letterStats.Add(latter, letterStat);
                        }

                        IncStatistic(ref letterStat);
                        letterStats[latter] = letterStat;
                    }
                }

                previousChar = currentChar;
            }

            return letterStats.Values.ToList();
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
             // TODO : Удалить статистику по запрошенному типу букв.
            switch (charType)
            {
                case CharType.Consonants:
                    letters = letters.Where(x => !x.IsVowel()).ToList();
                    break;
                case CharType.Vowel:
                    letters = letters.Where(x => x.IsVowel()).ToList();
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
        private static void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            letters = letters.OrderBy(x => x.Letter);

            foreach (var item in letters)
            {
                Console.WriteLine($"{item.Letter} : {item.Count}");
            }

            var sumCount = letters.Sum(x => x.Count);
            Console.WriteLine($"ИТОГО: {sumCount}");
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
