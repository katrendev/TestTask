using System;
using System.Linq;
using System.Collections.Generic;
using static TestTask.Vowel;

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
            IList<LetterStats> singleLetterStats;
            IList<LetterStats> doubleLetterStats;

            using (IReadOnlyStream inputStream = GetInputStream(args[0]))
            {
                singleLetterStats = FillSingleLetterStats(inputStream);
            }
            using (IReadOnlyStream inputStream = GetInputStream(args[1]))
            {
                doubleLetterStats = FillDoubleLetterStats(inputStream);
            }

            RemoveCharStatsByType(singleLetterStats, CharType.Consonants);
            RemoveCharStatsByType(doubleLetterStats, CharType.Vowel);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

            Console.ReadKey();

            // TODO : Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
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
            IDictionary<char, LetterStats> stats = new Dictionary<char, LetterStats>();

            stream.ResetPositionToStart();

            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (!Char.IsLetter(c))
                {
                    continue;
                }

                IncStatistic(stats, c);

                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
            }

            return stats.Values.ToList();
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
            IDictionary<char, LetterStats> stats = new Dictionary<char, LetterStats>();
            char prev = '!';
            bool rewind = false;

            stream.ResetPositionToStart();

            while (!stream.IsEof)
            {
                char current = Char.ToLower(stream.ReadNextChar());

                if (rewind)
                {
                    rewind = false;
                } 
                else if (Char.IsLetter(current) && Char.IsLetter(prev))
                {
                   if (current == prev)
                   {
                        IncStatistic(stats, current);
                        rewind = true;
                   }
                }

                prev = current;
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.
            }

            return stats.Values.ToList();
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
                    foreach(LetterStats stat in letters.Reverse())
                    {
                        if (!IsVowel(stat.Letter)) {
                            letters.Remove(stat);
                        }
                    }
                    break;
                case CharType.Vowel:
                    foreach (LetterStats stat in letters.Reverse())
                    {
                        if (IsVowel(stat.Letter))
                        {
                            letters.Remove(stat);
                        }
                    }
                    break;
                case CharType.None:
                    return;
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
            // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
            int sum = 0;
            foreach (LetterStats el in letters.OrderBy(el => el.Letter))
            {
                Console.WriteLine(el.Letter + " : " + el.Count);
                sum += el.Count;
            }
            Console.WriteLine("Итого: " + sum);
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(IDictionary<char, LetterStats> stats, char c)
        {
            if (stats.TryGetValue(c, out LetterStats value))
            {
                value.Count++;
            }
            else
            {
                stats[c] = new LetterStats(c.ToString(), 1);
            }
        }

    }
}
