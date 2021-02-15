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
            args = new string[] { "TestFiles\\File1.txt", "TestFiles\\File2.txt" };
            if (args.Length != 2)
            {
                Console.WriteLine("Задано неверное количество аргументов, укажите 2 аргумента, где:" +
                    Environment.NewLine + "1. Путь до первого файла" +
                    Environment.NewLine + "2. Путь до второго файла");
            }
            else
            {
                try
                {
                    using (IReadOnlyStream inputStream1 = GetInputStream(args[0]))
                    using (IReadOnlyStream inputStream2 = GetInputStream(args[1]))
                    {
                        IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
                        IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

                        RemoveCharStatsByType(ref singleLetterStats, CharType.Vowel);
                        RemoveCharStatsByType(ref doubleLetterStats, CharType.Consonants);

                        Console.WriteLine("Статистика по регистрозависимым одиночным буквам:" + Environment.NewLine);
                        PrintStatistic(singleLetterStats);
                        Console.WriteLine("===================================================================");
                        Console.WriteLine("Статистика по не регистрозависимым парным буквам:" + Environment.NewLine);
                        PrintStatistic(doubleLetterStats);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            // Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
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
            List<LetterStats> list = new List<LetterStats>();
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (char.IsLetter(c))
                {
                    int index = list.FindIndex(x => x.Letter == c.ToString());
                    if (index == -1)
                        list.Add(new LetterStats { Letter = c.ToString(), Count = 1 });
                    else
                    {
                        LetterStats stats = list[index];

                        // заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
                        IncStatistic(ref stats);
                        list[index] = stats;
                    }
                }
            }
            return list;
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
            List<LetterStats> list = new List<LetterStats>();
            stream.ResetPositionToStart();
            char tempChar = stream.ReadNextChar();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (char.IsLetter(c) && tempChar.ToString().ToLower() == c.ToString().ToLower())
                {
                    string newPair = "" + tempChar + c;
                    int index = list.FindIndex(x => x.Letter.ToLower() == newPair.ToLower());
                    if (index == -1)
                        list.Add(new LetterStats { Letter = newPair, Count = 1 });
                    else
                    {
                        LetterStats stats = list[index];

                        // заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.
                        IncStatistic(ref stats);
                        list[index] = stats;
                    }
                }
                tempChar = c;
            }
            return list;
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
            // Удалить статистику по запрошенному типу букв.
            string vowels = "aeiouyаоуэыяёюеи";
            switch (charType)
            {
                case CharType.Consonants:
                    letters = letters.Where(
                        x => x.Letter.ToLower().All(
                            z => vowels.Contains(z.ToString()))).ToList();
                    break;
                case CharType.Vowel:
                    letters = letters.Where(
                        x => !x.Letter.ToLower().All(
                            z => vowels.Contains(z.ToString()))).ToList();
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
            // Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
            var orderedArray = letters.OrderBy(x => x.Letter);
            foreach (var item in orderedArray)
                Console.WriteLine($"{item.Letter} : {item.Count}");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{Environment.NewLine}ИТОГО:{Environment.NewLine}{orderedArray.Sum(x => x.Count)} найденных букв/пар");
            Console.ResetColor();
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
