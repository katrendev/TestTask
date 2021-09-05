using System;
using System.Collections.Generic;
using System.IO;
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
        public static void Main(string[] args)
        {
            var errorMessage = IsValidParams(args);
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                IList<LetterStats> singleLetterStats;
                IList<LetterStats> doubleLetterStats;

                using (var inputStream1 = new ReadOnlyStream(args[0]))
                using (var inputStream2 = new ReadOnlyStream(args[1]))
                {
                    singleLetterStats = FillSingleLetterStats(inputStream1);
                    doubleLetterStats = FillDoubleLetterStats(inputStream2);
                }

                RemoveCharStatsByType(singleLetterStats, CharType.Vowels);
                RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

                PrintStatistic(singleLetterStats);
                PrintStatistic(doubleLetterStats);
            }
            else
            {
                Console.WriteLine(errorMessage + "\n");
            }

            Console.WriteLine("Нажмите любую клавишу для выхода");
            Console.ReadLine();
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            List<LetterStats> letters = new List<LetterStats>();
            stream.ResetPositionToStart();

            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (char.IsLetter(c))
                {
                    var stats = letters.FirstOrDefault(x => x.Letter == c.ToString());
                    if (stats == null)
                    {
                        stats = new LetterStats();
                        stats.Letter = c.ToString();
                        letters.Add(stats);
                    }

                    IncStatistic(stats);
                }
            }

            return letters;
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
            List<LetterStats> letters = new List<LetterStats>();
            stream.ResetPositionToStart();

            char currentChar = default;
            char nextChar;

            if (!stream.IsEof)
            {
                currentChar = stream.ReadNextChar();
            }

            while (!stream.IsEof)
            {
                if (char.IsLetter(currentChar))
                {
                    nextChar = stream.ReadNextChar();
                    if (char.IsLetter(nextChar))
                    {
                        if (char.ToLowerInvariant(currentChar) == char.ToLowerInvariant(nextChar))
                        {
                            var combinedChars = currentChar.ToString() + nextChar.ToString();
                            var stats = letters.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Letter) && x.Letter.ToLowerInvariant() == combinedChars.ToLowerInvariant());
                            if (stats == null)
                            {
                                stats = new LetterStats();
                                stats.Letter = combinedChars;
                                letters.Add(stats);
                            }

                            IncStatistic(stats);
                        }
                        else
                        {
                            currentChar = nextChar;
                            nextChar = default;
                        }
                    }
                }
                else
                {
                    currentChar = char.ToLowerInvariant(stream.ReadNextChar());
                }
            }

            return letters;
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
            switch (charType)
            {
                case CharType.Consonants:
                    var consonants = "бвгджзйклмнпрстфхцчшщ";
                    var removedConsonantsItems = letters.Where(x => !string.IsNullOrWhiteSpace(x.Letter) && consonants.Contains(x.Letter[0].ToString().ToLowerInvariant()));

                    foreach (var item in removedConsonantsItems)
                    {
                        letters.Remove(item);
                    }
                    break;
                case CharType.Vowels:
                    var vowels = "аеёиоуыэюя";
                    var removedVowelItems = letters
                        .Where(x => !string.IsNullOrWhiteSpace(x.Letter) && vowels.Contains(x.Letter[0].ToString().ToLowerInvariant()))
                        .ToList();

                    foreach (var item in removedVowelItems)
                    {
                        letters.Remove(item);
                    }
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
            var lettersOrder = letters.OrderBy(x => x.Letter);
            foreach (var letter in lettersOrder)
            {
                Console.WriteLine($"{{{letter.Letter}}} : {{{letter.Count}}}");
            }

            Console.WriteLine($"Итого: общее кол-во найденных букв/пар {letters.Sum(x => x.Count)} \n");
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной статистике.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
        }

        /// <summary>
        /// Проверка входных параметров при запуске программы
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Сообщение об ошибке</returns>
        private static string IsValidParams(string[] args)
        {
            if (args == null)
            {
                return "Не заданы входные параметры";
            }

            if (args.Length != 2)
            {
                return "Не верное кол-во входных параметров. Ожидается 2 параметра";
            }

            var path1 = args[0];
            var path2 = args[1];

            if (!File.Exists(path1) || !File.Exists(path2))
            {
                return $"Один из файлов не найден в системе.\n path1: {path1} \n {path2}";
            }

            if (Path.GetExtension(path1) != ".txt" || Path.GetExtension(path2) != ".txt")
            {
                return $"У одного из файлов не верное расширение.\n path1: {path1} \n {path2}";
            }

            return null;
        }
    }
}
