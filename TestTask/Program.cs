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
            IReadOnlyStream inputStream1 = GetInputStream(args[0]);
            IReadOnlyStream inputStream2 = GetInputStream(args[1]);

            Dictionary<string, int> singleLetterStats = FillSingleLetterStats(inputStream1);
            Dictionary<string, int> doubleLetterStats = FillDoubleLetterStats(inputStream2);

            RemoveCharStatsByType(ref singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(ref doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

            inputStream1.Dispose();
            inputStream2.Dispose();

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
        private static Dictionary<string, int> FillSingleLetterStats(IReadOnlyStream stream)
        {
            stream.ResetPositionToStart();
            var letterStats = new Dictionary<string, int>();

            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                IncLetterIntoDictionary(ref letterStats, c.ToString());
            }

            return letterStats;
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static Dictionary<string, int> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            stream.ResetPositionToStart();
            var letterStats = new Dictionary<string, int>();

            char[] pair = new char[2];
            pair[0] = stream.ReadNextChar();
            while (!stream.IsEof)
            {
                pair[1] = stream.ReadNextChar();

                if (char.ToUpper(pair[0]) == char.ToUpper(pair[1]))
                {
                    IncLetterIntoDictionary(ref letterStats, string.Concat(pair[0], pair[1]).ToString());
                }
                pair[0] = pair[1];
            }
            return letterStats;
        }


        /// <summary>
        /// Добавляет пару ключ значение в словарь, если такой не существует, а в противном
        /// случае, добавляет к количеству вхождений единицу
        /// </summary>
        /// <param name="values">Словарь - статистика</param>
        /// <param name="key">Вхождение символа или пары</param>
        private static void IncLetterIntoDictionary(ref Dictionary<string, int> letterStats, string key)
        {
            if (!letterStats.ContainsKey(key))
            {
                letterStats.Add(key, 1);
            }
            else letterStats[key] += 1;
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(ref Dictionary<string, int> letters, CharType charType)
        {
            char[] vovels = new char[] { 'а', 'е', 'ё', 'и', 'о', 'у', 'ы', 'э', 'ю', 'я', 'a', 'e', 'i', 'o', 'u', 'y' };
            switch (charType)
            {
                case CharType.Consonants:
                    {
                        letters = letters.Where(l => vovels.Contains(l.Key[0])).ToDictionary(l => l.Key, l => l.Value);
                    }
                    break;
                case CharType.Vowel:
                    {
                        letters = letters.Where(l => !vovels.Contains(l.Key[0]) && !int.TryParse(l.Key[0].ToString(), out int x)).ToDictionary(l => l.Key, l => l.Value);
                    }
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
        private static void PrintStatistic(Dictionary<string, int> letters)
        {
            foreach (var item in letters.OrderBy(l => l.Key))
            {
                Console.WriteLine($"{{{item.Key}}} : {{{item.Value}}}");
            }
            Console.WriteLine($"ИТОГО {{{letters.Count}}} : {{{letters.Sum(l => l.Value)}}}");
        }
    }
}
