using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

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
#if DEBUG
            args = new string[2];
            args[0] = @"C:\Work\TestTask\README.md";
            args[1] = @"C:\Work\TestTask\README.md";
#endif

            var inputStream1 = GetInputStream(args[0]);
            var inputStream2 = GetInputStream(args[1]);

            var singleLetterStats = FillSingleLetterStats(inputStream1);
            var doubleLetterStats = FillDoubleLetterStats(inputStream2);

            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

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
            List<LetterStats> result = new List<LetterStats>();
            var wordReg = new Regex(@"[A-Zа-я]", RegexOptions.IgnoreCase);
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                string symbol = Convert.ToString(stream.ReadNextChar(), CultureInfo.CreateSpecificCulture("ru-ru"));
                if (!wordReg.IsMatch(symbol)) continue;

                for (int ii = 0; ii < result.Count; ii++)
                {
                    
                    if (result[ii].Letter != symbol) continue;
                    var tmp = result[ii];
                    IncStatistic(ref tmp);
                    result[ii] = tmp;
                    symbol = string.Empty;
                    break;
                }

                
                if (string.IsNullOrEmpty(symbol))
                {
                    continue;
                }

                result.Add(new LetterStats() {Letter = symbol, Count = 1 });
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
        private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            List<LetterStats> result = new List<LetterStats>();
            var wordReg = new Regex(@"[A-Zа-я]{2}", RegexOptions.IgnoreCase);
            stream.ResetPositionToStart();
            char symbolPrevious = stream.ReadNextChar();
            while (!stream.IsEof)
            {
                char symbol = stream.ReadNextChar();
                var doubleLetter = $"{symbolPrevious}{symbol}";
                symbolPrevious = symbol;

                if (!wordReg.IsMatch(doubleLetter)) continue;

                for (int ii = 0; ii < result.Count; ii++)
                {
                    if (result[ii].Letter != doubleLetter) continue;

                    var tmp = result[ii];
                    IncStatistic(ref tmp);
                    result[ii] = tmp;

                    doubleLetter = string.Empty;
                    break;
                }

                if (string.IsNullOrEmpty(doubleLetter)) continue;

                result.Add(new LetterStats() { Letter = doubleLetter, Count = 1 });
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
        private static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            if (letters == null) return;

            Regex wordReg;
            switch (charType)
            {
                case CharType.Consonants:
                    wordReg = new Regex(@"[^аоиеёэыуюяAEIOU]", RegexOptions.IgnoreCase);
                    break;
                case CharType.Vowel:
                    wordReg = new Regex(@"[аоиеёэыуюяAEIOU]", RegexOptions.IgnoreCase);
                    break;
                default:
                    return;
            }
            
            for(var i = 0;i < letters.Count; i++)
            {
                var flContinue = true;
                for (var letter = 0; letter < letters[i].Letter.Length; letter++ )
                {
                    if (!wordReg.IsMatch(letters[i].Letter.Substring(letter, 1)))
                    {
                        flContinue = false;
                        break;
                    }
                }
                
                if (flContinue) continue;
                
                letters.Remove(letters[i]);
                i--;
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
            if (letters == null)
            {
                Console.WriteLine("буквы не найдены :(");
                return;
            }
            
            
            Console.WriteLine("\tБуква\t-\tКол-во");
            Console.WriteLine("\t------\t|\t-------");
            var letterCount = 0;
            var countSum = 0;
            foreach (var letter in letters.OrderBy(x => x.Letter))
            {
                Console.WriteLine($"\t{letter.Letter}\t:\t{letter.Count}");
                letterCount++;
                countSum += letter.Count;
            }
            Console.WriteLine("\t------\t|\t-------");
            Console.WriteLine($"Итого:\t{letterCount}\t : \t{countSum}",ConsoleColor.Green);
            //Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
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
