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
            IReadOnlyStream inputStream1 = GetInputStream(args[0]);
            IReadOnlyStream inputStream2 = GetInputStream(args[1]);

            IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
            IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

            RemoveCharStatsByType(ref singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(ref doubleLetterStats, CharType.Consonants);

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
            stream.ResetPositionToStart();

            Dictionary<char, LetterStats> dicLetterStats = new Dictionary<char, LetterStats>();

            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();

                if (!dicLetterStats.TryGetValue(c, out var letterStats))
                {
                    letterStats.Count = 0;
                    letterStats.Letter = c.ToString();
                    dicLetterStats.Add(c, letterStats);
                }

                var currentLetterStats = dicLetterStats[c];
                IncStatistic(ref currentLetterStats);
                dicLetterStats[c] = currentLetterStats;
            }

            return dicLetterStats.Values.ToList();
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

            Dictionary<string, LetterStats> dicLetterStats = new Dictionary<string, LetterStats>();

            while (!stream.IsEof)
            {
                var firstChar = string.Empty;

                if (string.IsNullOrEmpty(firstChar))
                {
                    firstChar = stream.ReadNextChar().ToString().ToUpper();
                }

                var secondChar = stream.ReadNextChar().ToString().ToUpper();

                if (secondChar.Equals(firstChar))
                {
                    var charPair = firstChar + secondChar;

                    if (!dicLetterStats.TryGetValue(charPair.ToString(), out var letterStats))
                    {
                        letterStats.Count = 0;
                        letterStats.Letter = charPair;
                        dicLetterStats.Add(charPair, letterStats);
                    }

                    var currentLetterStats = dicLetterStats[charPair];
                    IncStatistic(ref currentLetterStats);
                    dicLetterStats[charPair] = currentLetterStats;
                }

                firstChar = secondChar;
            }

            return dicLetterStats.Values.ToList();
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
            switch (charType)
            {
                case CharType.Consonants:
                    var latinConsonants = "bcdfghjklmnpqrstvwxzBCDFGHJKLMNPQRSTVWXZ";
                    var cyrillicConsonants = "бвгджзклмнпрстфхцчшщБВГДЖЗКЛМНПРСТФХЦЧШЩ";
                    var consonants = latinConsonants + cyrillicConsonants;
                    var listWithoutConsonants = letters.Where(l => !consonants.Contains(l.Letter.First())).ToList();
                    letters = listWithoutConsonants;
                    break;

                case CharType.Vowel:
                    var latinVowel = "AEIOUYaeiouy";
                    var cyrillicVowel = "аеёиоуыэюяАЕЁИОУЫЭЮЯ";
                    var vowel = latinVowel + cyrillicVowel;
                    var listWithoutVowel = letters.Where(l => !vowel.Contains(l.Letter.First())).ToList();
                    letters = listWithoutVowel;
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
            var sortedLetters = letters.OrderBy(l => l.Letter);
            var count = 0;

            foreach (LetterStats s in sortedLetters)
            {
                Console.WriteLine($"{{{s.Letter}}} : {{{s.Count}}}");
                count += s.Count;
            }

            Console.WriteLine($"{{ИТОГО}} : {{{count}}}");
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
