using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;

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

        private static readonly List<LetterStats> SingleLetterList = new List<LetterStats>();

        private static readonly List<LetterStats> DoubleLetterList = new List<LetterStats>();

        private static readonly char[] ConsonantLetters = { 'B', 'C', 'D', 'F', 'G', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'S', 'T', 'V', 'X', 'Z', 'H', 'R', 'W', 'Y', 'Б', 'В', 'Г', 'Д', 'Ж', 'З', 'Й', 'К', 'Л', 'М', 'Н', 'П', 'Р', 'С', 'Т', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ' };
        private static readonly char[] VowelLetters = { 'A', 'E', 'I', 'O', 'U', 'W', 'Y', 'А', 'И', 'Е', 'Ё', 'О', 'У', 'Ы', 'Э', 'Ю', 'Я', 'a', 'e', 'i', 'o', 'u', 'w', 'y', 'а', 'и', 'е', 'ё', 'о', 'у', 'ы', 'э', 'ю', 'я' };

        static void Main(string[] args)
        {
            var inputStream1 = GetInputStream(args[0]);
            var inputStream2 = GetInputStream(args[1]);

            var singleLetterStats = FillSingleLetterStats(inputStream1);
            var doubleLetterStats = FillDoubleLetterStats(inputStream2);

            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

            Console.WriteLine("Press any key to exit");
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
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                var c = stream.ReadNextChar();
                if (!char.IsLetter(c))
                {
                    continue;
                }

                LetterStats letter;
                if (SingleLetterList.Exists(x => x.Letter == c.ToString()))
                {
                    letter = SingleLetterList.Find(x => x.Letter == c.ToString());
                    SingleLetterList.Remove(letter);
                    IncStatistic(ref letter);
                }
                else
                {
                    letter = new LetterStats { Count = 1, Letter = c.ToString() };
                }

                SingleLetterList.Add(letter);

            }

            stream.ResetPositionToStart();
            return SingleLetterList;
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
            while (!stream.IsEof)
            {
                var c1 = stream.ReadNextChar();
                if (!char.IsLetter(c1))
                {
                    continue;
                }

                var c2 = stream.ReadNextChar();
                if (!char.IsLetter(c2))
                {
                    continue;
                }

                var c = $"{c1}{c2}";

                LetterStats letter;
                if (DoubleLetterList.Exists(x => x.Letter == c))
                {
                    letter = DoubleLetterList.Find(x => x.Letter == c);
                    DoubleLetterList.Remove(letter);
                    IncStatistic(ref letter);
                }
                else
                {
                    letter = new LetterStats { Count = 1, Letter = c };
                }

                DoubleLetterList.Add(letter);

            }

            stream.Dispose();
            return DoubleLetterList;
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
                    foreach (var letter in ConsonantLetters)
                    {
                        var exists = letters.FirstOrDefault(x => x.Letter.Contains(letter.ToString()));
                        if (!exists.Equals(default))
                        {
                            letters.Remove(exists);
                        }
                    }
                    break;
                case CharType.Vowel:
                    foreach (var letter in VowelLetters)
                    {
                        var exists = letters.FirstOrDefault(x => x.Letter == letter.ToString());
                        if (!exists.Equals(default))
                        {
                            letters.Remove(exists);
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
            // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
            var table = new Table();
            table.AddColumn(nameof(LetterStats.Letter));
            table.AddColumn(nameof(LetterStats.Count));

            foreach (var letter in letters.OrderByDescending(x=>x.Count))
            {
                table.AddRow(letter.Letter, letter.Count.ToString());
            }

            AnsiConsole.Write(table);
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
