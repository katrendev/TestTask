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
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Not enough arguments!");
                return;
            }
            if (!File.Exists(args[0]) || !File.Exists(args[1]))
            {
                Console.WriteLine("Wrong file path!");
                return;
            }
            using (var inputStream1 = GetInputStream(args[0]))
            {
                using (var inputStream2 = GetInputStream(args[1]))
                {
                    var singleLetterStats = FillSingleLetterStats(inputStream1);
                    var doubleLetterStats = FillDoubleLetterStats(inputStream2);

                    RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                    RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

                    PrintStatistic(singleLetterStats);
                    PrintStatistic(doubleLetterStats);

                    Console.ReadKey();
                }
            }
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
            IList<LetterStats> answer = new List<LetterStats>();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (!char.IsLetter(c)) continue;
                int i;
                for (i = 0; i < answer.Count; i++)
                {
                    if (answer[i].Letter[0] == c)
                    {
                        answer[i] = IncStatistic(answer[i]);
                        break;
                    }
                }

                if (i == answer.Count)
                {
                    var newStruct = new LetterStats();
                    newStruct.Letter = c.ToString();
                    newStruct.Count = 1;
                    answer.Add(newStruct);
                }
            }

            return answer;
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
            var accumulator = new List<char>();
            IList<LetterStats> answer = new List<LetterStats>();
            var accPosition = 0;
            while (!stream.IsEof)
            {
                while (accPosition < 2)
                {
                    accumulator.Add(stream.ReadNextChar());
                    accPosition++;
                }

                var leftChar = Char.ToLower(accumulator[0]);
                var rightChar = Char.ToLower(accumulator[1]);

                if (leftChar.Equals(rightChar))
                {
                    int i;
                    for (i = 0; i < answer.Count; i++)
                    {
                        if (answer[i].Letter == string.Concat(leftChar, rightChar))
                        {
                            answer[i] = IncStatistic(answer[i]);
                            break;
                        }
                    }

                    if (i == answer.Count)
                    {
                        var newStruct = new LetterStats
                            {Letter = string.Concat(leftChar, rightChar), Count = 1};
                        answer.Add(newStruct);
                    }
                }

                accumulator.RemoveAt(0);
                accPosition--;
            }

            return answer;
        }

        private static void RemoveAll<T>(IList<T> list, Predicate<T> match)
        {
            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (match(list[i]))
                {
                    list.RemoveAt(i);
                }
            }
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
            bool IsThisCharType(LetterStats c)
            {
                switch (charType)
                {
                    case CharType.Vowel:
                        return "aeiouAEIOU".IndexOf(c.Letter[0]) >= 0;
                    case CharType.Consonants:
                        return "aeiouAEIOU".IndexOf(c.Letter[0]) < 0;
                }

                return false;
            }

            RemoveAll(letters, IsThisCharType);
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
            var letterStats = letters.ToList();
            var sorted = letterStats.OrderBy(ls => ls.Letter);
            foreach (var ls in sorted)
            {
                Console.WriteLine($"{ls.Letter} : {ls.Count}");
            }

            Console.WriteLine(letterStats.Sum(ls => ls.Count));
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static LetterStats IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
            return letterStats;
        }
    }
}