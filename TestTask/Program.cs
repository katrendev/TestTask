using System;
using System.Collections.Generic;
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
            if (args.Length < 2) return;
            IReadOnlyStream inputStream1 = GetInputStream(args[0]);
            IReadOnlyStream inputStream2 = GetInputStream(args[1]);

            List<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
            List<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

            singleLetterStats=RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            doubleLetterStats=RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

            Console.ReadKey(false);
            // TODO : Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
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
        private static List<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            List<LetterStats> letterStatsSingleCollections= new List<LetterStats>();
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                letterStatsSingleCollections= AddLetterStats(""+ stream.ReadNextChar(), letterStatsSingleCollections);
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
            }
            //return ???;
            return letterStatsSingleCollections;
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static List<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            List<LetterStats> letterStatsDoubleCollections= new List<LetterStats>();
            string previousSymbol="";
            stream.ResetPositionToStart();

            while (!stream.IsEof)
            {
                string c =""+ stream.ReadNextChar();
                
                if(String.Compare(c.ToLower(), previousSymbol.ToLower())==0)
                {
                    letterStatsDoubleCollections = AddLetterStats(c.ToLower() + previousSymbol.ToLower(), letterStatsDoubleCollections);
                }
                previousSymbol = c;
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.
            }

            //return ???;
            return letterStatsDoubleCollections;
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static List<LetterStats> RemoveCharStatsByType(List<LetterStats> letters, CharType charType)
        {
            // TODO : Удалить статистику по запрошенному типу букв.
            switch (charType)
            {
                case CharType.Consonants:
                    {
                        letters = letters.Where(x => !Regex.IsMatch(x.Letter, "[бвгджзйклмнпрстфхцчшщqwrtypsdfghjklzxcvbnm]", RegexOptions.IgnoreCase)).ToList();
                        break;
                    }
                case CharType.Vowel:
                    {
                        letters = letters.Where(x => !Regex.IsMatch(x.Letter, "[аеёиоуыэяюaouei]", RegexOptions.IgnoreCase)).ToList();
                        break;
                    }
            }
            return letters;
            
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(List<LetterStats> letters)
        {
            // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!

    letters.Sort(delegate (LetterStats x, LetterStats y)
            {
                if (x.Letter == null && y.Letter == null) return 0;
                else if (x.Letter == null) return -1;
                else if (y.Letter == null) return 1;
                else return x.Letter.CompareTo(y.Letter);
            });
            foreach(LetterStats print in letters)
            {
                Console.WriteLine(print.Letter + "  " + print.Count);
            }
        
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
        }
        
        private static List<LetterStats> AddLetterStats(string letter,List<LetterStats> list)
        {
            if (Char.IsLetter(letter[0]))
            {
                if (list.Count > 0 && list.Exists(x => x.Letter == letter))
                {
                    IncStatistic(list.Find(x => x.Letter == letter));
                    return list;
                }
                list.Add(new LetterStats(letter));
            }
                return list;
            
        }

    }
}
