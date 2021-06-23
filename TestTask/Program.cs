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
            if(args.Length != 2)
            {
                Console.WriteLine("Введено неверное количество аргументов, допустимо только 2");
                return;
            }

            try
            {
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
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine("Для выхода из программы нажмите любую клавишу...");
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
            List<LetterStats> stats = new List<LetterStats>();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (char.IsLetter(c))
                {
                    string letter = c.ToString();
                    var index = stats.FindIndex(x => string.Compare(x.Letter, letter) == 0);
                    if (index == -1)
                    {
                        var stat = new LetterStats
                        {
                            Letter = letter,
                            Count = 1
                        };
                        stats.Add(stat);
                    }
                    else
                    {
                        var stat = stats[index];
                        IncStatistic(ref stat);
                        stats[index] = stat;
                    }
                }
            }

            return stats;
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
            List<LetterStats> stats = new List<LetterStats>();
            stream.ResetPositionToStart();
            char firstLetter = '\0';
            bool firstLetterReaded = false;
            while (!stream.IsEof)
            {
                if(!firstLetterReaded)
                    firstLetter = char.ToUpper(stream.ReadNextChar());
                if (stream.IsEof)
                    break;
                char secondLetter = char.ToUpper(stream.ReadNextChar());
                if (char.IsLetter(firstLetter) && char.IsLetter(secondLetter) && firstLetter == secondLetter)
                {
                    firstLetterReaded = false;
                    var pair = firstLetter.ToString() + secondLetter;
                    var index = stats.FindIndex(x => string.Compare(x.Letter, pair) == 0);
                    if (index == -1)
                    {
                        var stat = new LetterStats
                        {
                            Letter = pair,
                            Count = 1
                        };
                        stats.Add(stat);
                    }
                    else
                    {
                        var stat = stats[index];
                        IncStatistic(ref stat);
                        stats[index] = stat;
                    }
                    
                }
                else
                {
                    if (char.IsLetter(secondLetter))
                    {
                        firstLetterReaded = true;
                        firstLetter = secondLetter;
                    }
                }
            }

            return stats;
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
                    for(int i = 0; i < letters.Count;)
                    {
                        var ch = letters[i].Letter[0];
                        if (ch.IsVowel())
                            i++;
                        else
                            letters.RemoveAt(i);
                    }
                    break;
                case CharType.Vowel:
                    for (int i = 0; i < letters.Count;)
                    {
                        var ch = letters[i].Letter[0];
                        if (ch.IsVowel())
                            letters.RemoveAt(i);
                        else
                            i++;
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
            Console.WriteLine("Статистика букв:");
            if(!(letters?.Any() ?? false))
            {
                Console.WriteLine("Статистика отсутствует или пуста");
                return;
            }
            var sortedLetters = letters.OrderBy(x => x.Letter);
            foreach(var letter in sortedLetters)
            {
                Console.WriteLine("{0} : {1}", letter.Letter, letter.Count);
            }
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
