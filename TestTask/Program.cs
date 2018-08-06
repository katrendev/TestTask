using System;
using System.Collections.Generic;
using System.Collections;
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

            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

            // TODO : Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
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
            using (stream)
            {
                stream.ResetPositionToStart();

                List<LetterStats> result = new List<LetterStats>();

                while (!stream.IsEof)
                {
                    char c = stream.ReadNextChar();

                    // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
                    LetterStats stat = result.Find(x => x.Letter == c.ToString());

                    if (stat == null)
                    {
                        stat = new LetterStats(c.ToString());
                        result.Add(stat);
                    }

                    IncStatistic(stat);
                }

                return result;
            }
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
            using (stream)
            {
                stream.ResetPositionToStart();

                char? lastChar = null;

                List<LetterStats> result = new List<LetterStats>();

                while (!stream.IsEof)
                {
                    char c = stream.ReadNextChar();

                    // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.

                    if (lastChar.ToString().ToLower() == c.ToString().ToLower())
                    {
                        LetterStats stat = result.Find(x => x.Letter == c.ToString().ToLower());

                        if (stat == null)
                        {
                            stat = new LetterStats(c.ToString().ToLower());
                            result.Add(stat);
                        }

                        IncStatistic(stat);

                        lastChar = null;
                    }
                    else
                        lastChar = c;

                }

                return result;
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
            for (int i = 0; i < letters.Count; i++)
            {
                // TODO : Удалить статистику по запрошенному типу букв.

                if (CharTypeDetect.DetectType(letters[i]) == charType)
                    letters.Remove(letters[i]);
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
            var sortLetters = letters.OrderBy(l => l.Letter);

            foreach (LetterStats ls in sortLetters)
                Console.WriteLine(ls.Letter + " : " + ls.Count);

            Console.WriteLine("Итого " + letters.Sum(x => x.Count));

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
