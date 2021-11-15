using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    public class Program
    {
        /// <summary>
        /// Гласные буквы для метода RemoveCharStatsByType (английские и русские)
        /// </summary>
        private static readonly string[] VowelSymbols = { "a", "e", "o", "i", "y", "u", "э", "и", "ы", "ё", "ю", "я", "а", "е", "о", "у" };

        /// <summary>
        /// Программа принимает на входе 2 пути до файлов.
        /// Анализирует в первом файле кол-во вхождений каждой буквы (регистрозависимо). Например А, б, Б, Г и т.д.
        /// Анализирует во втором файле кол-во вхождений парных букв (не регистрозависимо). Например АА, Оо, еЕ, тт и т.д.
        /// По окончанию работы - выводит данную статистику на экран.
        /// </summary>
        /// <param name="args">Первый параметр - путь до первого файла.
        /// Второй параметр - путь до второго файла.</param>
        private static void Main(string[] args)
        {
            var inputStream1 = GetInputStream(args[0]);
            var inputStream2 = GetInputStream(args[1]);

            var singleLetterStats = FillLetterStats(inputStream1, true);
            var doubleLetterStats = FillLetterStats(inputStream2, false);

            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

            inputStream1.CloseToFile();
            inputStream2.CloseToFile();

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
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <param name="isCheckLetterCase">Учитывать ли регистр</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillLetterStats(IReadOnlyStream stream, bool isCheckLetterCase)
        {
            var cursorCurrentPosition = 0;
            stream.ResetPositionToStart();
            var listStats = new List<LetterStats>();

            while (!stream.IsEof)
            {
                var symbol = stream.ReadNextChar(cursorCurrentPosition++);

                if (symbol is null) continue;

                var letter = isCheckLetterCase
                    ? listStats.FirstOrDefault(x => x.Letter[0] == symbol[0])
                    : listStats.FirstOrDefault(x => x.Letter.ToLower()[0] == symbol.ToLower()[0]);

                if (!(letter is null))
                    IncStatistic(letter);
                else
                    listStats.Add(isCheckLetterCase
                    ? new LetterStats(symbol, 1)
                    : new LetterStats(symbol.ToUpper() + symbol.ToLower(), 1));
            }

            return listStats;
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
            switch (charType)
            {
                case CharType.Consonants:
                    for (var i = 0; i < letters.Count; i++)
                    {
                        var a = letters[i].Letter[0].ToString().ToLower();

                        if (!VowelSymbols.Contains(a))
                        {
                            letters.RemoveAt(i);
                            i--;
                        }
                    }
                    break;
                case CharType.Vowel:
                    for (var i = 0; i < letters.Count; i++)
                    {
                        var a = letters[i].Letter[0].ToString().ToLower();

                        if (VowelSymbols.Contains(a))
                        {
                            letters.RemoveAt(i);
                            i--;
                        }
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
        private static void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            var result = letters.OrderBy(x => x.Letter);

            foreach (var letter in result) Console.WriteLine($"{letter.Letter}: {letter.Count}");

            Console.WriteLine($"Итого количество найденных букв: {letters.Count()}");
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