using System;
using System.Collections.Generic;

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

                RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

                Console.WriteLine("Single Letter Statistic:\r\n");
                PrintStatistic(singleLetterStats);
                Console.WriteLine("\r\n-------------------------\r\n\r\nDouble Letter Statistic:\r\n");
                PrintStatistic(doubleLetterStats);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (inputStream1 != null) inputStream1.Dispose();
                if (inputStream2 != null) inputStream2.Dispose();
            }

            Console.WriteLine("\r\nPress Any Key to Exit...");
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
            string alphabet = "abcdefghijklmnopqrstuvwxyzабвгдеёжзийклмнопрстуфхцчшщьыъэюя";
            Dictionary<char, int> pos = new Dictionary<char, int>();
            IList<LetterStats> result = new List<LetterStats>();

            foreach (char ch in alphabet)
            {
                result.Add(new LetterStats(ch));
                result.Add(new LetterStats(char.ToUpper(ch)));
                pos[ch] = result.Count - 2;
                pos[char.ToUpper(ch)] = result.Count - 1;
            }

            stream.ResetPositionToStart();

            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.

                if (!char.IsLetter(c)) continue;

                IncStatistic(result[pos[c]]);
            }

            for (int i = result.Count - 1; i >= 0; i--)
                if (result[i].Count == 0) result.RemoveAt(i);

            return result;
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
            char c0 = '\0';
            string alphabet = "abcdefghijklmnopqrstuvwxyzабвгдеёжзийклмнопрстуфхцчшщьыъэюя";
            Dictionary<char, int> pos = new Dictionary<char, int>();
            IList<LetterStats> result = new List<LetterStats>();

            foreach (char ch in alphabet)
            {
                result.Add(new LetterStats($"{ch}{ch}".ToUpper()));
                pos[ch] = result.Count - 1;
            }

            stream.ResetPositionToStart();

            while (!stream.IsEof)
            {
                char c = char.ToLower(stream.ReadNextChar());
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.

                if (!char.IsLetter(c))
                {
                    c0 = '\0';
                    continue;
                }

                if (c0 == c)
                    IncStatistic(result[pos[c]]);
                else
                    c0 = c;
            }

            for (int i = result.Count - 1; i >= 0; i--)
                if (result[i].Count == 0) result.RemoveAt(i);

            return result;
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
                    for (int i = letters.Count - 1; i >= 0; i--)
                        if (!letters[i].IsVowel) letters.RemoveAt(i);
                    break;
                case CharType.Vowel:
                    for (int i = letters.Count - 1; i >= 0; i--)
                        if (letters[i].IsVowel) letters.RemoveAt(i);
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
            // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
            foreach (var item in letters)
                Console.WriteLine($"{item.Letter} : {item.Count}");
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
