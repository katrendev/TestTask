using System;
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
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("Необходимо передать пути к файлам");
            }



            using (IReadOnlyStream inputStream1 = GetInputStream(args[0]))
            {
                using(IReadOnlyStream inputStream2 = GetInputStream(args[1]))
                {
                    IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
                    IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

                    RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                    RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

                    PrintStatistic(singleLetterStats);
                    PrintStatistic(doubleLetterStats);
                }
            }

            // TODO : Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
            Console.WriteLine("Press any key for continue");
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
            var letterStats = new List<LetterStats>();
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                var c = stream.ReadNextChar();
                if (!char.IsLetter(c))
                {
                    continue;
                }
                var letter = c.ToString();

                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
                var stat = letterStats.FirstOrDefault(x => x.Letter == letter);
                if (stat.Count == 0)
                {
                    letterStats.Add(new LetterStats
                    {
                        Letter = letter,
                        Count = 1
                    });
                }
                else
                {
                    letterStats.Remove(stat);
                    IncStatistic(ref stat);
                    letterStats.Add(stat);
                }
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
        private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            var letterStats = new List<LetterStats>();

            stream.ResetPositionToStart();
            var previousLetter = string.Empty;

            while (!stream.IsEof)
            {
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.
                var c = stream.ReadNextChar();
                if (!char.IsLetter(c))
                {
                    previousLetter = string.Empty;
                    continue;
                }

                var letter = c.ToString().ToLower();
                if (previousLetter != letter)
                {
                    previousLetter = letter;
                    continue;
                }

                var stat = letterStats.FirstOrDefault(x => x.Letter == previousLetter + letter);
                if (stat.Count == 0)
                {
                    letterStats.Add(new LetterStats
                    {
                        Letter = previousLetter + letter,
                        Count = 1
                    });
                }
                else
                {
                    letterStats.Remove(stat);
                    IncStatistic(ref stat);
                    letterStats.Add(stat);
                }
                previousLetter = letter;
            }

            return letterStats;
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
                    for(var index = letters.Count - 1; index >= 0; index--)
                    {
                        if (letters[index].Letter.ContainsOnlyConsonants())
                        {
                            letters.RemoveAt(index);
                        }
                    }
                    break;
                case CharType.Vowel:
                    for (var index = letters.Count - 1; index >= 0; index--)
                    {
                        if (letters[index].Letter.ContainsOnlyVowels())
                        {
                            letters.RemoveAt(index);
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
            var sortedLetters = letters.OrderBy(x => x.Letter);
            foreach(var letter in sortedLetters)
            {
                Console.WriteLine($"{letter.Letter} : {letter.Count}");
            }
            Console.WriteLine($"Итого: {letters.Count()}");
            Console.WriteLine();
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
