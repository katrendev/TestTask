using System;
using System.Collections.Generic;
using System.IO;
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
            if (args.Length < 2)
            {
                Console.WriteLine("Ошибка: Необходимо указать пути до двух файлов в качестве аргументов.");
                return;
            }

            string fileName1 = args[0];
            string fileName2 = args[1];

            if (!File.Exists(fileName1) || !File.Exists(fileName2))
            {
                Console.WriteLine($"Ошибка: Файл '{fileName1}' или '{fileName2}' не найден(ы).");
                return;
            }

            using (IReadOnlyStream inputStream1 = GetInputStream(fileName1))
            using (IReadOnlyStream inputStream2 = GetInputStream(fileName2))
            {
                IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
                IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

                RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

                PrintStatistic(singleLetterStats);
                PrintStatistic(doubleLetterStats);
            }

            Console.WriteLine("Программа завершена. Нажмите любую клавишу для выхода...");
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
            var letterStatsList = new List<LetterStats>();

            while (!stream.IsEof)
            {
                try
                {
                    char currentChar = stream.ReadNextChar();
                    if (char.IsLetter(currentChar))
                    {
                        int index = letterStatsList.FindIndex(ls => ls.Letter == currentChar.ToString());
                        if (index >= 0)
                        {
                            var updatedLetterStat = letterStatsList[index];
                            IncStatistic(ref updatedLetterStat);
                            letterStatsList[index] = updatedLetterStat;
                        }
                        else letterStatsList.Add(new LetterStats { Letter = currentChar.ToString(), Count = 1 });
                    }
                }
                catch (EndOfStreamException)
                {
                    break;
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
            var letterStatsList = new List<LetterStats>();
            char? previousChar = null;

            while (!stream.IsEof)
            {
                try
                {
                    char currentChar = stream.ReadNextChar();
                    char lowerChar = char.ToLower(currentChar);

                    if (previousChar.HasValue && lowerChar == char.ToLower(previousChar.Value))
                    {
                        string pairKey = $"{lowerChar}{lowerChar}";
                        int index = letterStatsList.FindIndex(ls => ls.Letter == pairKey);
                        if (index >= 0)
                        {
                            var updatedLetterStat = letterStatsList[index];
                            IncStatistic(ref updatedLetterStat);
                            letterStatsList[index] = updatedLetterStat;
                        }
                        else letterStatsList.Add(new LetterStats { Letter = pairKey, Count = 1 });
                    }
                    previousChar = lowerChar;
                }
                catch (EndOfStreamException)
                {
                    break;
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
        private static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            for (int i = letters.Count - 1; i >= 0; i--)
            {
                var letter = letters[i].Letter;
                if (ShouldRemove(letter, charType)) letters.RemoveAt(i);
            }
        }

        private static bool ShouldRemove(string letter, CharType charType)
        {
            var vowels = new HashSet<char> { 'а', 'е', 'ё', 'и', 'о', 'у', 'ы', 'э', 'ю', 'я' };
            var consonants = new HashSet<char> { 'б', 'в', 'г', 'д', 'ж', 'з', 'й', 'к', 'л', 'м', 'н', 'п', 'р', 'с', 'т', 'ф', 'х', 'ц', 'ч', 'ш', 'щ' };
            char lowerLetter = char.ToLower(letter[0]);

            switch (charType)
            {
                case CharType.Vowel:
                    return vowels.Contains(lowerLetter);
                case CharType.Consonants:
                    return consonants.Contains(lowerLetter);
                default:
                    return false;
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
            var sortedLetters = letters.OrderBy(l => l.Letter).ToList();
            int totalCount = 0;
            foreach (var letterStat in sortedLetters)
            {
                Console.WriteLine($"Буква: {letterStat.Letter}, Количество: {letterStat.Count}");
                totalCount += letterStat.Count;
            }
            Console.WriteLine($"ИТОГО : {totalCount}");
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(ref LetterStats letterStats)
        {
            letterStats.Count++;
        }
    }
}
