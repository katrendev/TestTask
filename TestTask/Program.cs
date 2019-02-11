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
            IReadOnlyStream inputStream1 = null;
            IReadOnlyStream inputStream2 = null;

            try
            {
                inputStream1 = GetInputStream(args[0]);
                inputStream2 = GetInputStream(args[1]);

                IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
                IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

                RemoveCharStatsByType(ref singleLetterStats, CharType.Vowel);
                RemoveCharStatsByType(ref doubleLetterStats, CharType.Consonants);

                PrintStatistic(singleLetterStats);
                PrintStatistic(doubleLetterStats);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                inputStream1?.Dispose();
                inputStream2?.Dispose();
            }

            // TODO : Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
            Console.Write("\nPress any key...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        private static IReadOnlyStream GetInputStream(string fileFullPath)
        {
            if (fileFullPath == null)
                throw new ArgumentNullException("Empty file path!");

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
            List<LetterStats> letterStatsList = new List<LetterStats>();

            while (!stream.IsEof)
            {
                char ch = stream.ReadNextChar();
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
                if (char.IsLetter(ch))
                {
                    IncStatistic(ch.ToString(), letterStatsList);
                }
            }
            return letterStatsList;
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
            List<LetterStats> letterStatsList = new List<LetterStats>();
            List<char> charStats = new List<char>();

            while (!stream.IsEof)
            {
                char ch = char.ToUpper(stream.ReadNextChar());
                if (char.IsLetter(ch))
                {
                    // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.
                    if (charStats.Contains(ch))
                    {
                        charStats.Remove(ch);
                        IncStatistic(string.Concat(ch, ch), letterStatsList);
                    }
                    else
                    {
                        charStats.Add(ch);
                    }
                }
            }

            return letterStatsList;
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
            // TODO : Удалить статистику по запрошенному типу букв.
            var result = from member in letters
                         where member.Type != charType
                         select member;

            letters = result.ToList();
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
            var result = letters.OrderBy(x => x.Letter);
            Console.WriteLine("Статистика:");
            foreach (var member in result)
            {
                Console.WriteLine($"{{{member.Letter}}} : {{{member.Count}}}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStatsList"></param>
        private static void IncStatistic(string chStr, List<LetterStats> letterStatsList)
        {
            for (int i = 0; i < letterStatsList.Count; i++)
            {
                if (letterStatsList[i].Letter == chStr)
                {
                    // если нашли соответсвие, то увеличиваем и выходим из функции
                    letterStatsList[i] = new LetterStats(chStr, letterStatsList[i].Count + 1);
                    return;
                }
            }

            // если не нашли такой элемент, то добавляем его
            letterStatsList.Add(new LetterStats(chStr, 1));
        }
    }
}
