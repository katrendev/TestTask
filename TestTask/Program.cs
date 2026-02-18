using System;
using System.Collections.Generic;
using System.IO;

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
            // если пути не были переданы, запрашиваем у пользователя
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: TestTask <file1> <file2>");
                return;
            }

            using (IReadOnlyStream inputStream1 = GetInputStream(args[0]))
            using (IReadOnlyStream inputStream2 = GetInputStream(args[1]))
            {
                IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
                IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

                RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

                PrintStatistic(singleLetterStats);
                PrintStatistic(doubleLetterStats);
            }

            // дождаться нажатия любой клавиши перед закрытием
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey(true);
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
            var stats = new Dictionary<string, LetterStats>();

            stream.ResetPositionToStart();
            while (true)
            {
                if (stream.IsEof)
                    break;

                char c;
                try
                {
                    c = stream.ReadNextChar();
                }
                catch (EndOfStreamException)
                {
                    // достигли конца - просто прерываем цикл
                    break;
                }

                if (!char.IsLetter(c))
                    continue;

                string key = c.ToString();
                if (!stats.TryGetValue(key, out LetterStats letterStats))
                {
                    letterStats = new LetterStats { Letter = key, Count = 0 };
                }

                IncStatistic(ref letterStats); // регистрозависимо используем сам символ
                stats[key] = letterStats;
            }

            return new List<LetterStats>(stats.Values);
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
            // используем словарь с игнорированием регистра для ключей
            var stats = new Dictionary<string, LetterStats>(StringComparer.OrdinalIgnoreCase);

            stream.ResetPositionToStart();
            char? prevChar = null;
            while (true)
            {
                if (stream.IsEof)
                    break;

                char c;
                try
                {
                    c = stream.ReadNextChar();
                }
                catch (EndOfStreamException)
                {
                    break;
                }

                if (!char.IsLetter(c))
                {
                    prevChar = null;
                    continue;
                }

                if (prevChar.HasValue)
                {
                    // сравниваем без учёта регистра
                    if (char.ToUpperInvariant(prevChar.Value) == char.ToUpperInvariant(c))
                    {
                        string key = new string(new[] { char.ToUpperInvariant(c), char.ToUpperInvariant(c) });
                        if (!stats.TryGetValue(key, out LetterStats letterStats))
                        {
                            letterStats = new LetterStats { Letter = key, Count = 0 };
                        }

                        IncStatistic(ref letterStats);
                        stats[key] = letterStats;
                    }
                }

                prevChar = c;
            }

            return new List<LetterStats>(stats.Values);
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
            // Удаляем элементы из списка по префиксу соответствующему типу
            bool IsVowelChar(char ch)
            {
                // базовый набор русских гласных (и их латинские аналоги, если встретятся)
                const string vowels = "AEIOUYАЕЁИОУЫЭЮЯaeiouyаеёиоуыэюя";
                return vowels.IndexOf(ch) >= 0;
            }

            for (int i = letters.Count - 1; i >= 0; i--)
            {
                string str = letters[i].Letter;
                if (string.IsNullOrEmpty(str))
                    continue;

                bool allVowels = true;
                bool allConsonants = true;
                foreach (char ch in str)
                {
                    if (!char.IsLetter(ch))
                        continue; // игнорируем несравнимые символы
                    if (IsVowelChar(ch))
                        allConsonants = false;
                    else
                        allVowels = false;
                }

                bool remove = charType == CharType.Vowel ? allVowels : allConsonants;
                if (remove)
                    letters.RemoveAt(i);
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
            var sorted = new List<LetterStats>(letters);
            sorted.Sort((a, b) => string.CompareOrdinal(a.Letter, b.Letter));

            int total = 0;
            foreach (var ls in sorted)
            {
                Console.WriteLine($"{ls.Letter} : {ls.Count}");
                total += ls.Count;
            }

            Console.WriteLine($"ИТОГО : {total}");
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// Принципиально принимаем структуру по ссылке, чтобы изменения
        /// увидел вызывающий код (struct - value type).
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(ref LetterStats letterStats)
        {
            letterStats.Count++;
        }


    }
}
