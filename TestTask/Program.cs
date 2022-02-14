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
            using (var inputStream1 = GetInputStream(args[0]))
            {
                var singleLetterStats = FillSingleLetterStats(inputStream1);
                RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                Console.WriteLine("Статистика вхождения каждой буквы регистрозависимо, исключая гласные:");
                PrintStatistic(singleLetterStats);
            }
            using (var inputStream2 = GetInputStream(args[1]))
            {
                var doubleLetterStats = FillDoubleLetterStats(inputStream2);
                RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);
                Console.WriteLine("Статистика вхождения парных букв нерегистрозависимо, исключая согласные:");
                PrintStatistic(doubleLetterStats);
            }
            Console.WriteLine("Нажмите Enter...");
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
        private static List<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            var listLetterStats = new List<LetterStats>();
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                var nextChar = stream.ReadNextChar();
                if (!char.IsLetter(nextChar)) continue;
                var stats = listLetterStats.FirstOrDefault(x => x.Letter == nextChar.ToString());
                if (stats?.Letter != null)
                    stats.Count += 1;
                else
                    listLetterStats.Add(new LetterStats { Letter = nextChar.ToString(), Count = 1 });
            }
            return listLetterStats;
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static List<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            var listLetterStats = new List<LetterStats>();
            var previousChar = "";
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                var nextChar = stream.ReadNextChar();
                if (char.IsLetter(nextChar))
                {
                    var letter = nextChar.ToString().ToLower();
                    if (letter == previousChar)
                    {
                        var pairLetters = string.Concat(letter, previousChar);
                        var stats = listLetterStats.FirstOrDefault(x => x.Letter == pairLetters);
                        if (stats?.Letter != null)
                            stats.Count += 1;
                        else
                            listLetterStats.Add(new LetterStats { Letter = pairLetters, Count = 1 });
                    }
                    previousChar = letter;
                }
            }
            return listLetterStats;
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(List<LetterStats> letters, CharType charType)
        {
            var vowels = new List<string> { "у", "е", "ы", "а", "о", "э", "я", "и", "ю", "ё" };
            switch (charType)
            {
                case CharType.Consonants:
                    letters.RemoveAll(x => !vowels.Exists(v => v == x.Letter.ToLower().Substring(0, 1)));
                    break;
                case CharType.Vowel:
                    letters.RemoveAll(x => vowels.Exists(v => v == x.Letter.ToLower().Substring(0, 1)));
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
        private static void PrintStatistic(List<LetterStats> letters)
        {
            var sortedLetters = letters.OrderBy(x => x.Letter).ToList();
            foreach (var letter in sortedLetters)
                Console.WriteLine("{0} : {1}", letter.Letter, letter.Count);
        }
    }
}
