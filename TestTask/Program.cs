using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
            {
                ShowMessage("Запуск программы: TestTask.exe <file1> <file2>");
                return;
            }

            for (int i = 0; i <= 1; i++)
            {
                if (!System.IO.File.Exists(args[i]))
                {
                    ShowMessage("Файл " + args[i] + " не найден.");
                    return;
                }
            }

            IList<LetterStats> singleLetterStats = FillSingleLetterStats(args[0]);
            IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(args[1]);

            RemoveCharStatsByType(ref singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(ref doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

            ShowMessage("Вывод статистики закончен");
            // TODO : Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
        }


        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="fileName">Полный путь до файла для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillSingleLetterStats(string fileName)
        {
            char[] result = ReadFile(fileName);

            IList<LetterStats> statsList = new List<LetterStats>();
            foreach (char c in result)
            {
                if (char.IsLetter(c))
                {
                    LetterStats stats = new LetterStats();
                    if (statsList.Where(s => s.Letter.Equals(c.ToString(), StringComparison.Ordinal)).Any())
                    {
                        LetterStats stats2 = statsList.Where(s => s.Letter.Equals(c.ToString(), StringComparison.Ordinal)).First();
                        stats = stats2;
                        statsList.Remove(stats2);
                    }
                    else
                    {
                        stats.Letter = c.ToString();
                    }
                    IncStatistic(ref stats);
                    // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
                    statsList.Add(stats);
                }
            }

            return statsList;

            throw new NotImplementedException();
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="fileName">Полный путь до файла для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillDoubleLetterStats(string fileName)
        {
            char[] result = ReadFile(fileName);

            IList<LetterStats> statsList = new List<LetterStats>();
            for (int i = 0; i < result.Length - 1; i++)
            {
                char c1 = result[i];
                char c2 = result[i + 1];
                if (char.IsLetter(c1) && char.IsLetter(c2) && c1.ToString().ToUpper() == c2.ToString().ToUpper())
                {
                    LetterStats stats = new LetterStats();
                    string c = c1.ToString() + c2.ToString();
                    if (statsList.Where(s => s.Letter.Equals(c, StringComparison.OrdinalIgnoreCase)).Any())
                    {
                        LetterStats stats2 = statsList.Where(s => s.Letter.Equals(c, StringComparison.OrdinalIgnoreCase)).First();
                        stats = stats2;
                        statsList.Remove(stats2);
                    }
                    else
                    {
                        stats.Letter = c;
                    }
                    IncStatistic(ref stats);
                    // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.
                    statsList.Add(stats);
                }
            }

            return statsList;

            throw new NotImplementedException();
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
            const string consonants= "БВГДЖЗЙКЛМНПРСТФХЦЧШЩЪЬ"; // уточнить надо ли ЙЪЬ
            const string vowel = "АЕЁИОУЫЭЮЯ";
            string strtodel = "";
            IList<LetterStats> lettersResult = new List<LetterStats>();

            // TODO : Удалить статистику по запрошенному типу букв.
            switch (charType)
            {
                case CharType.Consonants:
                    strtodel = consonants;
                    break;
                case CharType.Vowel:
                    strtodel = vowel;
                    break;
            }

            foreach(LetterStats let in letters)
            {
                if (strtodel.IndexOf(let.Letter.Substring(0, 1).ToUpper()) < 0)
                    lettersResult.Add(let);
            }

            letters = lettersResult;
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
            int total = 0;
            foreach (LetterStats let in letters.OrderBy(c => c.Letter.ToUpper()))
            {
                Console.WriteLine("{0} : {1}", let.Letter, let.Count);
                total += let.Count;
            }
            Console.WriteLine("---------");
            Console.WriteLine("ИТОГО: {0}\n\n", total);
            // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!

            return;

            throw new NotImplementedException();
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
        /// Метод считывает входной файл.
        /// </summary>
        /// <param name="fileName">Полный путь до файла для считывания символов</param>
        private static char[] ReadFile(string fileName)
        {
            char[] result;
            using (StreamReader reader = new StreamReader(fileName, Encoding.Default))
            {
                result = new char[reader.BaseStream.Length];
                reader.Read(result, 0, (int)reader.BaseStream.Length);
            }
            return result;
        }

        /// <summary>
        /// Метод выводит сообщение на консоль и делает паузу.
        /// </summary>
        /// <param name="message">Сообщение</param>
        private static void ShowMessage(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine("\n\nНажмите любую клавишу для завершения программы...");
            Console.ReadKey();
        }

    }
}
