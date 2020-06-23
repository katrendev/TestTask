using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
                Console.WriteLine("Не указаны пути к файлам");
                Console.ReadKey(false);
                return;
            }
                
            IList<LetterStats> singleLetterStats;
            IList<LetterStats> doubleLetterStats;
            using (IReadOnlyStream inputStream1 = GetInputStream(args[0]))
            {
                singleLetterStats = FillSingleLetterStats(inputStream1);
            }
            using (IReadOnlyStream inputStream2 = GetInputStream(args[1]))
            {
                doubleLetterStats = FillDoubleLetterStats(inputStream2);
            }

            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

            // TODO : DONE Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
            Console.ReadKey(false);
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
            var letterstats = new List<LetterStats>();
            while (!stream.IsEof)
            {
                string c = stream.ReadNextChar().ToString();
                LetterStats ls = letterstats.FirstOrDefault(x => x.Letter == c);
                if (ls == null)
                {
                    letterstats.Add(new LetterStats() { Letter = c, Count = 1 });
                }
                else IncStatistic(ls);
                // TODO : DONE заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
            }
            return letterstats;
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
            var letterstats = new List<LetterStats>();
            string c;
            if (!stream.IsEof) c = stream.ReadNextChar().ToString();
            else return letterstats;
            while (!stream.IsEof)
            {
                string cnext = stream.ReadNextChar().ToString();
                if (c.ToLower() == cnext.ToLower())
                {
                    Console.WriteLine($"{c} {cnext}");
                    LetterStats ls = letterstats.FirstOrDefault(x => x.Letter.ToLower() == (c + cnext).ToLower());
                    if (ls == null)
                    {
                        letterstats.Add(new LetterStats() { Letter = (c + cnext), Count = 1 });
                    }
                    else IncStatistic(ls);
                }
                c = cnext;
                // TODO : DONE заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.
            }
            return letterstats;
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
            // TODO : DONE Удалить статистику по запрошенному типу букв.
            switch (charType)
            {
                case CharType.Consonants:
                    {
                        letters = letters.Where(x => !Regex.IsMatch(x.Letter, "[бвгджзйклмнпрстфхцчшщ]", RegexOptions.IgnoreCase)).ToList();
                        break;
                    }
                case CharType.Vowel:
                    {
                        letters = letters.Where(x => !Regex.IsMatch(x.Letter, "[аеёиоуыэюя]", RegexOptions.IgnoreCase)).ToList();
                        break;
                    }
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
            // TODO : DONE Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
            letters.OrderBy(x => x.Letter);
            foreach (var letter in letters)
            {
                Console.WriteLine($"{{{letter.Letter}}} : {{{letter.Count}}}");
            }
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
        }
    }
}
