using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TestTask
{
    /// <summary>
    /// Стандартный класс, содержащий точку входа.
    /// </summary>
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
            if (args.Length != 0)
            {
                IReadOnlyStream inputStream1 = GetInputStream(args[0]);
                IReadOnlyStream inputStream2 = GetInputStream(args[1]);

                IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
                IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

                IList<LetterStats> RemoveVowelChars = RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                IList<LetterStats> RemoveConsonantsChars = RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);
                //Вывод на экрна исходных статистик и после удаления завленных типов букв.
                {
                    PrintStatistic(singleLetterStats);
                    PrintStatistic(doubleLetterStats);
                    Console.WriteLine("Удаление гласных");
                    PrintStatistic(RemoveVowelChars);

                    Console.WriteLine("Удаление согласных");
                    PrintStatistic(RemoveConsonantsChars);
                }
            }
            else Console.WriteLine("Не заданы аргументы командной строки.");
            // TODO : Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
            Console.ReadKey();
        }

        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения.</param>
        /// <returns>Поток для последующего чтения.</returns>
        private static IReadOnlyStream GetInputStream(string fileFullPath) => new ReadOnlyStream(fileFullPath);
        
        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            List<LetterStats> stats = new List<LetterStats>();
            LetterStats ReadChar = new LetterStats();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (char.IsLetter(c))
                {                  
                    ReadChar.Letter = c.ToString();
                    ReadChar.Count = (stream as ReadOnlyStream).NumberOfOccurrences(c);
                    /* TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
                     - IncStatistic - не задействовал. Логика подсчета инкапсулирована в классе ReadOnlyStream.
                     - Коды символов отличаются в зависимости от регистра, доп. действий не требуется. */
                    stats.Add(ReadChar);
                }             
            }
            (stream as ReadOnlyStream).FindPairLetter();
            // закрываем входящий поток.
            stream.Dispose();                            
            return stats;
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
            List<LetterStats> stats = new List<LetterStats>();
            LetterStats ReadChar = new LetterStats();
            foreach (string item in (stream as ReadOnlyStream).FindPairLetter())
            {
                ReadChar.Letter = item;
                ReadChar.Count = (stream as ReadOnlyStream).NumberOfOccurrences(item);
                stats.Add(ReadChar);
            }
            stream.Dispose();
            return stats;
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар.</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static IList<LetterStats> RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            List<LetterStats> PatStats = new List<LetterStats>();
            LetterStats LetterPatStats = new LetterStats();
            string PatternVowel = "[АаЕеЁёИиОоУуЫыЭэЮюЯя]";
            string patternConsonants = "[БбВвГгДдЖжЗзЙйКкЛлМмНнПпРрСсТтФфХхЦцЧчШшЩщ]";
            string str = string.Empty;
            // TODO : Удалить статистику по запрошенному типу букв.
            switch (charType)
            {
                case CharType.Consonants:
                    foreach (var item in letters.ToList().Distinct())
                    {
                        str = Regex.Replace(item.Letter, patternConsonants, "");
                        if (str.Length > 0)
                        {
                            LetterPatStats.Letter = str;
                            LetterPatStats.Count = item.Count;
                            PatStats.Add(LetterPatStats);
                        }
                    }
                    break;
                case CharType.Vowel:
                    foreach (var item in letters.ToList().Distinct())
                    {
                        str = Regex.Replace(item.Letter, PatternVowel, "");
                        if (str.Length > 0)
                        {
                            LetterPatStats.Letter = str;
                            LetterPatStats.Count = item.Count;
                            PatStats.Add(LetterPatStats);
                        }
                    }
                    break;
            }
            return PatStats;
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
            int count = 0;
            try
            {
                // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
                letters.ToList().Sort();
                foreach (LetterStats item in letters.Distinct())
                {
                    Console.WriteLine(item.ToString());
                    count++;
                }
            }
            catch (ArgumentNullException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            Console.WriteLine($"Итого найденных букв/пар - {count}\n");
        }
    }
}
