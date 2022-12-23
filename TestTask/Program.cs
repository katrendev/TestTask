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

            if (singleLetterStats != null)
            {
                RemoveCharStatsByType(ref singleLetterStats, CharType.Vowel);
            }

            if (doubleLetterStats != null)
            {
                RemoveCharStatsByType(ref doubleLetterStats, CharType.Consonants);
            }

            Console.WriteLine("Статистика вхождения символов");

            if (singleLetterStats != null)
            {
                PrintStatisticSingle(singleLetterStats);
            }

            if (doubleLetterStats != null)
            {
                PrintStatisticDouble(doubleLetterStats);
            }

            // TODO : Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
            Console.WriteLine("Нажмите любую клавишу для завершения работы");
            Console.ReadLine();
        }

        /// <summary>
        /// Функция возвращает экземпляр потока с уже загруженным файлом для чтения символов.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        private static IReadOnlyStream GetInputStream(string fileFullPath)
        {
            return new ReadOnlyStream(fileFullPath);
        }

        /// <summary>
        /// Функция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            Dictionary<string, LetterStats> LetterStatsContainer = new Dictionary<string, LetterStats>();

            char[] charsFromFile = stream.ReadAllChar();

            if (charsFromFile != null)
            {
                for (int charArrayIndex = 0; charArrayIndex < charsFromFile.Length; charArrayIndex++)
                {
                    char symbol = charsFromFile[charArrayIndex];

                    if (char.IsLetter(symbol))
                    {
                        IncStatistic(ref LetterStatsContainer, symbol.ToString());
                    }
                }

                IList<LetterStats> letterStats = LetterStatsContainer.Select(container => container.Value).ToList();
                return letterStats;
            }

            return null;
        }

        /// <summary>
        /// Функция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            Dictionary<string, LetterStats> LetterStatsContainer = new Dictionary<string, LetterStats>();

            char[] charsFromFile = stream.ReadAllChar();

            if (charsFromFile != null)
            {
                for (int charArrayIndex = 0; charArrayIndex < charsFromFile.Length - 1; charArrayIndex++)
                {
                    char symbol1 = charsFromFile[charArrayIndex];
                    char symbol2 = charsFromFile[charArrayIndex + 1];

                    if (char.IsLetter(symbol1) && char.IsLetter(symbol2))
                    {
                        if (symbol1.ToString().ToLower() == symbol2.ToString().ToLower())
                        {
                            string key = symbol1.ToString().ToLower() + symbol2.ToString().ToLower();
                            IncStatistic(ref LetterStatsContainer, key);
                        }
                    }
                }
                IList<LetterStats> letterStats = LetterStatsContainer.Select(container => container.Value).ToList();
                return letterStats;
            }

            return null;
        }

        /// <summary>
        /// Функция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(ref IList<LetterStats> letters, CharType charType)
        {
            string consonants = "аеёиоуыэюяaeiouy";
            // TODO : Удалить статистику по запрошенному типу букв.
            switch (charType)
            {
                case CharType.Consonants:
                    letters = letters.Where(letter => !consonants.Contains(letter.Letter.ToLower()[0])).ToList();
                    break;
                case CharType.Vowel:
                    letters = letters.Where(letter => consonants.Contains(letter.Letter.ToLower()[0])).ToList();
                    break;
            }
        }

        /// <summary>
        /// Функция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatisticSingle(IEnumerable<LetterStats> letters)
        {
            // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
            int total = 0;
            letters.OrderBy(letter => letter.Letter);

            Console.WriteLine("Статистика для отдельных букв");

            foreach (LetterStats letter in letters)
            {
                total += letter.Count;
                Console.WriteLine($"{letter.Letter} - {letter.Count}");
            }
            Console.WriteLine($"ИТОГО НАЙДЕНО: {total} букв(ы)");
        }

        /// <summary>
        /// Функция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatisticDouble(IEnumerable<LetterStats> letters)
        {
            // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
            int total = 0;
            letters.OrderBy(letter => letter.Letter);

            Console.WriteLine("Статистика для пар букв");

            foreach (LetterStats letter in letters)
            {
                total += letter.Count;
                Console.WriteLine($"{letter.Letter} - {letter.Count}");
            }
            Console.WriteLine($"ИТОГО НАЙДЕНО: {total} пар(ы)");
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по каждой букве/паре букв.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(ref Dictionary<string, LetterStats> LetterStatsContainer, string key)
        {
            if (LetterStatsContainer.ContainsKey(key))
            {
                LetterStats LetterStat = LetterStatsContainer[key];
                LetterStat.Count++;
                LetterStatsContainer[key] = LetterStat;
            }
            else
            {
                LetterStatsContainer.Add(key, new LetterStats(key, 1));
            }
        }
    }
}
