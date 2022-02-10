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
            if (args.Length != 2)
            {
                Console.WriteLine("Error: incorrect number of paths to files. Press any key to exit...");
                Console.ReadKey();
                return;
            }

            string singleLettersFilePath = args[0];
            string doubleLettersFilePath = args[1];

            try
            {
                using (IReadOnlyStream inputStreamSingleLetters = GetInputStream(singleLettersFilePath),
                                       inputStreamDoubleLetters = GetInputStream(doubleLettersFilePath))
                {
                    var singleLetterStats = FillSingleLetterStats(inputStreamSingleLetters);
                    var doubleLetterStats = FillDoubleLetterStats(inputStreamDoubleLetters);

                    RemoveCharStatsByType(ref singleLetterStats, CharType.Vowel);
                    RemoveCharStatsByType(ref doubleLetterStats, CharType.Consonants);

                    PrintResult(singleLetterStats, doubleLetterStats);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"The file could not be read: {e.Message}");
            }

            Console.WriteLine("Press any key to exit...");
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
            var listStats = new List<LetterStats>();
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                var currentChar = stream.ReadNextChar();
                if (char.IsLetter(currentChar))
                {
                    IncStatistic(currentChar.ToString(), listStats, true);
                }                
            }

            return listStats;
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
            var listStats = new List<LetterStats>();
            var previousChar = char.MinValue;
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                var currentChar = stream.ReadNextChar();
                if (char.IsLetter(currentChar))
                {
                    if (char.ToLowerInvariant(previousChar) == char.ToLowerInvariant(currentChar))
                    {
                        var str = string.Empty;
                        str += previousChar;
                        str += currentChar;
                        IncStatistic(str, listStats, false);
                        previousChar = char.MinValue;
                    }
                    else
                    {
                        previousChar = currentChar;
                    }
                }
            }

            return listStats;            
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
            var vowels = new char[5] { 'a', 'e', 'i', 'o', 'u' };
            switch (charType)
            {
                case CharType.Consonants:
                    letters = letters.Where(x => x.Letter.ToLowerInvariant().Any(c => vowels.Contains(c))).ToList();
                    break;
                case CharType.Vowel:
                    letters = letters.Where(x => !x.Letter.ToLowerInvariant().Any(c => vowels.Contains(c))).ToList();
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
        private static void PrintStatistic(IList<LetterStats> letters)
        {
            var sortedLettersList = letters.OrderBy(x => x.Letter).ToList();
            sortedLettersList.ForEach(i => Console.WriteLine(string.Format("{{{0}}} : {{{1}}}", i.Letter, i.Count)));
            Console.WriteLine(string.Format("Total items : {0}", letters.Count));
        }

        private static void PrintResult(IList<LetterStats> singleLetterStats, IList<LetterStats> doubleLetterStats)
        {
            Console.WriteLine("Single letter statistics:");
            PrintStatistic(singleLetterStats);
            Console.WriteLine("Double letter statistics:");
            PrintStatistic(doubleLetterStats);
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(string letter, List<LetterStats> listStats, bool isCaseSensitive)
        {
            int existingCharIndex;
            if (isCaseSensitive)
            {
                existingCharIndex = listStats.FindIndex(i => i.Letter == letter);              
            }
            else
            {
                existingCharIndex = listStats.FindIndex(i => i.Letter.ToLowerInvariant() == letter.ToLowerInvariant());
            }
            
            if (existingCharIndex != -1)
            {
                var existingCharCount = listStats.ElementAt(existingCharIndex).Count;
                listStats.RemoveAt(existingCharIndex);
                listStats.Add(new LetterStats(letter, ++existingCharCount));
            }
            else
            {
                listStats.Add(new LetterStats(letter, 1));
            }
        }
    }
}
