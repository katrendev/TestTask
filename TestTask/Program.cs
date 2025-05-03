using System;
using System.IO;
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
        /// 
        static void Main(string[] args)
        {
            if (args.Length != 2) throw new ArgumentException("Invalid argument count. It should contain paths to two files.");
            if (!File.Exists(args[0]) || !File.Exists(args[1])) throw new ArgumentException($"Make sure the specified files exist");

            IList<LetterStats> singleLetterStats;

            using (IReadOnlyStream inputStream1 = GetInputStream(args[0]))
            {
                singleLetterStats = FillSingleLetterStats(inputStream1);
            }

            IList<LetterStats> doubleLetterStats;

            using (IReadOnlyStream inputStream2 = GetInputStream(args[1]))
            {
                doubleLetterStats = FillDoubleLetterStats(inputStream2);
            }

            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

            Console.WriteLine("Press any key to exit.");
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
            stream.ResetPositionToStart();

            List<LetterStats> listLS = new List<LetterStats>();

            while (!stream.IsEof)
            {
                char ch = stream.ReadNextChar();

                if (!char.IsLetter(ch))
                    continue;

                CountEntry(listLS, ch.ToString());
            }
            return listLS;
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

            List<LetterStats> listLS = new List<LetterStats>();
            char? prevCh = null;

            while (!stream.IsEof)
            {
                char currCh = char.ToUpper(stream.ReadNextChar());

                if (!char.IsLetter(currCh))
                    continue;

                if (prevCh.HasValue && prevCh.Value == currCh)
                {
                    string tmp = $"{prevCh}{currCh}";

                    CountEntry(listLS, tmp);

                    prevCh = null;
                }
                else
                {
                    prevCh = currCh;
                }
            }
            return listLS;
        }

        /// <summary>
        /// Функция проверяет наличие передаваемой строки в списке, и либо добавляет новую запись, либо увеличивет счетчик у имеющейся
        /// </summary>
        /// <param name="list">Коллекция LetterStats в которой выполняется поиск </param>
        /// <param name="value">Строка для поиска в коллекции list</param>
        static void CountEntry(List<LetterStats> list, string value)
        {
            int idx = list.FindIndex(x => x.Letter == value);

            if (idx > -1)
            {
                IncStatistic(list[idx]);
            }
            else
            {
                list.Add(new LetterStats() { Letter = value, Count = 1 });
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
            for (int i = letters.Count - 1; i >= 0; i--)
            {
                char firstChar = letters[i].Letter[0];
                bool isVowel = vowels.Contains(firstChar);

                if ((charType == CharType.Vowel && isVowel) || (charType == CharType.Consonants && !isVowel))
                {
                    letters.RemoveAt(i);
                }
            }
        }
        /// <summary>
        /// Список гласных букв Русского и Английского алфавитов
        /// </summary>
        private static readonly HashSet<char> vowels = new HashSet<char>("аАеЕёЁиИоОуУыЫэЭюЮяЯaAeEiIoOuUyY".ToCharArray());

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            var sorted = letters.OrderBy(x => x.Letter).ToList(); //Метод не предусматривает изменение исходного списка, поэтому создается новый объект
            foreach (var item in sorted)
            {
                Console.WriteLine($"{item.Letter} : {item.Count}");
            }
            Console.WriteLine($"Итого: {sorted.Count}");
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
