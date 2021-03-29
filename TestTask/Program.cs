using System;
using System.Collections.Generic;
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
            //IReadOnlyStream inputStream1 = GetInputStream(args[0]);
            //ReadOnlyStream inputStream2 = GetInputStream(args[1]);
            const string testPath1 = "test1.txt";
            IReadOnlyStream inputStream1 = GetInputStream(testPath1);
            IReadOnlyStream inputStream2 = GetInputStream(testPath1);

            var singleLetterStats = FillSingleLetterStats(inputStream1);
            inputStream1.Close();
            var doubleLetterStats = FillDoubleLetterStats(inputStream2);
            inputStream2.Close();

            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats.Values);
            PrintStatistic(doubleLetterStats.Values);

            Console.Read();
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
        private static Dictionary<char, LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            stream.ResetPositionToStart();
            var result = new Dictionary<char, LetterStats>();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (!c.IsLetter())
                {
                    continue;
                }
                LetterStats stat;
                if (!result.ContainsKey(c))
                {
                    stat = new LetterStats(c);
                }
                else
                {
                    stat = result[c];
                }
                IncStatistic(ref stat);
                result[c] = stat;
            }

            return result;
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static Dictionary<char, LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            stream.ResetPositionToStart();
            var comparer = new MyCharIgnoreCaseComparer();
            var result = new Dictionary<char, LetterStats>(comparer);
            var buf = '\0';
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (!c.IsLetter())
                {
                    buf = c;
                    continue;
                }
                if (!comparer.Equals(c,buf))
                {
                    buf = c;
                    continue;
                }
                LetterStats stat;
                if (!result.ContainsKey(c))
                {
                    stat = new LetterStats(new StringBuilder().Append(buf).Append(c).ToString());
                }
                else
                {
                    stat = result[c];
                }
                IncStatistic(ref stat);
                result[c] = stat;
                buf = c;
            }

            return result;
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(Dictionary<char, LetterStats> letters, CharType charType)
        {
            foreach (var pair in letters
            .Where(x => x.Value.charsType == charType).ToArray())
                letters.Remove(pair.Key);
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
            var sorted = letters.OrderBy(x=>x.Letter,StringComparer.OrdinalIgnoreCase);
            foreach(var element in sorted)
            {
                Console.WriteLine($"{element.Letter} : {element.Count}");
            }
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(ref LetterStats letterStats)
        {
            letterStats++;
        }
    }
}
