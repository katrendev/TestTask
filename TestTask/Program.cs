using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestTask.Extensions;

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
            try
            {
                string fullPath1 = GetFilePath(args[0]);
                string fullPath2 = GetFilePath(args[1]);

                IList<LetterStats> singleLetterStats;
                IList<LetterStats> doubleLetterStats;

                using (IReadOnlyStream inputStream1 = GetInputStream(fullPath1),
                    inputStream2 = GetInputStream(fullPath2))
                {
                    singleLetterStats = FillSingleLetterStats(inputStream1);
                    doubleLetterStats = FillDoubleLetterStats(inputStream2);
                }

                RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

                PrintStatistic(singleLetterStats);
                PrintStatistic(doubleLetterStats);
            
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
            // TODO : Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
        }

        private static string GetFilePath(string path)
        {
            if (File.Exists(path))
            {
                return path;
            }
            else
            {
                throw new FileNotFoundException($"Файл не найден, путь до файла: {path}");
            }
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
            var result = new List<LetterStats>();

            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (char.IsLetter(c))
                {
                    LetterStats letterStats = result.FirstOrDefault(x => x.Letter.Equals(c.ToString()));

                    if (letterStats != null)
                    {
                        IncStatistic(letterStats);
                    }
                    else
                    {
                        letterStats = new LetterStats(c.ToString());
                        IncStatistic(letterStats);
                        result.Add(letterStats);
                    }

                }
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
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
            // С моей точки зрения из описания не совсем понятно как поступить с последовательностью в
            // которой пара содержит лишний символ, к примеру - *АаА*
            // Мной был выбран вариант при котором отсекается "третий лишний" и выполнение продолжается дальше
            // В итоге при входящей последовательности АаАБб будут добалены пары (Аа : 1, Бб : 1)
            
            var result = new List<LetterStats>();
            var previsionLetter = '\0';

            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (!char.IsLetter(c))
                {
                    continue;
                }

                if (previsionLetter.Equals(c, true))
                {
                    string resultLetter = $"{previsionLetter}{c}";

                    LetterStats letterStats = result.FirstOrDefault(
                        x => x.Letter.Equals(resultLetter, StringComparison.OrdinalIgnoreCase));

                    if (letterStats != null)
                    {
                        IncStatistic(letterStats);
                    }
                    else
                    {
                        letterStats = new LetterStats(resultLetter);
                        IncStatistic(letterStats);
                        result.Add(letterStats);
                    }

                    previsionLetter = '\0';
                }
                else
                {
                    previsionLetter = c;
                }
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.
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
            // TODO : Удалить статистику по запрошенному типу букв.
            switch (charType)
            {
                case CharType.Consonants:
                    letters.RemoveAll(x => x.Letter.ToCharArray().IsConsonant());
                    break;
                case CharType.Vowel:
                    letters.RemoveAll(x => x.Letter.ToCharArray().IsVowel());
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
            letters = letters.OrderBy(x => x.Letter);

            int totalLetterFound = 0;
            foreach (var letter in letters)
            {
                totalLetterFound += letter.Count;
                Console.WriteLine(letter);
            }
            Console.WriteLine($"Итого {totalLetterFound}");
            // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(LetterStats letterStats)
        {
            ++letterStats.Count;
        }
    }
}