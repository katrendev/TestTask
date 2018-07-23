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
            if (args.Count() > 0)
            {
                if (args.Count() >= 2)
                {
                    try
                    {
                        IReadOnlyStream inputStream1 = GetInputStream(args[0]);
                        IReadOnlyStream inputStream2 = GetInputStream(args[1]);

                        if (inputStream1 != null)
                        {
                            IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
                            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                            PrintStatistic(singleLetterStats);
                        }
                        Console.WriteLine();
                        if (inputStream2 != null)
                        {
                            IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);
                            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);
                            PrintStatistic(doubleLetterStats);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                    Console.Write("Файлов в командной строке должно быть два.");
            }
            else
                Console.Write("Не переданы пути файлов в командной строке.");

            // TODO : Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
            Console.Write("\nНажмите любую клавишу для выхода...");
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
        private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            List<LetterStats> lst = null;
            LetterStats ls;
            int index;
            char c;
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                c = stream.ReadNextChar();
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.

                if (char.IsLetter(c))
                {
                    if (lst == null)
                        lst = new List<LetterStats>();
                    ls = lst.FirstOrDefault(x => x.Letter == c.ToString());
                    index = lst.IndexOf(ls);
                    if (ls.Letter == null)
                    {
                        ls.Letter = c.ToString();
                        ls.Count = 1;
                        lst.Add(ls);
                    }
                    else
                        lst[index] = IncStatistic(ls);
                }
            }

            return lst;

            // throw new NotImplementedException();
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
            List<string> lstStr = new List<string>();
            List<LetterStats> lst = new List<LetterStats>();
            char c;
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                c = stream.ReadNextChar();
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.

                if (char.IsLetter(c))
                    lstStr.Add(c.ToString());
            }
            CharSearch(lstStr, lst);
            return lst;

            //throw new NotImplementedException();
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

            List<char> vowelsLellers = new List<char> { 'а', 'е', 'ё', 'и', 'о', 'у', 'ы', 'э', 'ю', 'я' };
            List<LetterStats> lst = new List<LetterStats>();

            // Удаляем статистику по запрошенному типу букв.
            // Можно удалять с помощью цикла while либо с помощью временного списка.
            switch (charType)
            {
                case CharType.Consonants:
                    lst.Clear();
                    foreach (var letter in letters)
                    {
                        if (!vowelsLellers.Contains(letter.Letter.ToLowerInvariant().ToCharArray()[0]))
                            lst.Add(letter);
                    }
                    break;
                case CharType.Vowel:
                    foreach (var letter in letters)
                    {
                        if (vowelsLellers.Contains(letter.Letter.ToLowerInvariant().ToCharArray()[0]))
                            lst.Add(letter);
                    }
                    break;
            }
            foreach (var letter in lst)
                letters.Remove(letter);
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

            // Выводим отсортированную статистику на экран.
            foreach (var letter in letters.OrderBy(x => x.Letter))
                Console.WriteLine($"{letter.Letter} : {letter.Count}");

            // throw new NotImplementedException();
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        /// Используется структура, тип значения.
        /// Для фиксации изменений возвращаем структуру
        private static LetterStats IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
            return letterStats;
        }

        // Дополнительный метод для выбора пар букв, с использованием рекурсии
        private static void CharSearch(IList<string> lstStr, IList<LetterStats> letters)
        {
            if (lstStr.Count > 0)
            {
                string s = lstStr.First();
                string s1;
                LetterStats ls;
                int index;
                int index1;
                lstStr.RemoveAt(0);
                if ((s1 = lstStr.FirstOrDefault(x => x.ToLower() == s.ToLower())) != null)
                {
                    index = lstStr.IndexOf(s1);
                    ls = letters.FirstOrDefault(x => x.Letter == (s + s1));
                    if (ls.Letter == null)
                    {
                        ls.Letter = s + s1;
                        ls.Count = 1;
                        letters.Add(ls);
                    }
                    else
                    {
                        index1 = letters.IndexOf(ls);
                        letters[index1] = IncStatistic(ls);
                    }
                    lstStr.RemoveAt(index);
                }
                CharSearch(lstStr, letters);
            }
        }
    }
}
