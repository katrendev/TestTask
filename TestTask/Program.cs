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
            ConsoleColor curFor = Console.ForegroundColor;

            IReadOnlyStream inputStream1 = null;
            IReadOnlyStream inputStream2 = null;
            try
            {
                inputStream1 = GetInputStream(args[0]);
                inputStream2 = GetInputStream(args[1]);
            } 
            catch (Exception E)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка при открытии потока: " + E.Message);
                Console.ForegroundColor = curFor;
            }
           
            List<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
            List<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

            int countSingleLetter = singleLetterStats.Count;
            int countDoubleLetter = doubleLetterStats.Count;

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Получено из потока одиночных букв: " + countSingleLetter);
            Console.WriteLine("Получено из потока парных букв: " + countDoubleLetter);

            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Удалено одиночных букв: " + (countSingleLetter - singleLetterStats.Count));
            Console.WriteLine("Удалено парных букв: " + (countDoubleLetter - doubleLetterStats.Count));

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\r\nСтатистика по одиночным буквам");
            Console.WriteLine(new String('-', 30));
            Console.ForegroundColor = curFor;

            PrintStatistic(singleLetterStats);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\r\nСтатистика по парам букв");
            Console.WriteLine(new String('-', 25));
            Console.ForegroundColor = curFor;

            PrintStatistic(doubleLetterStats);

            // TODO : Необходимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
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
        private static List<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            List<LetterStats> r = new List<LetterStats>();

            if (stream != null)
            {
                stream.ResetPositionToStart();
                while (!stream.IsEof)
                {
                    string c = stream.ReadNextChar().ToString();
                    // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый. 
                    int i = r.FindIndex(x => x.Letter.Equals(c, StringComparison.Ordinal));
                    if (i != -1)
                    {                        
                        r[i] = IncStatistic(r[i]);
                    } 
                    else
                    {
                        if (Char.IsLetter(c[0]))
                        {
                            r.Add(new LetterStats() { Letter = c, Count = 1 });
                        }
                    }
                }
            }

            return r;            
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
            List<LetterStats> r = new List<LetterStats>();            

            if (stream != null)
            {
                char nc = char.MinValue;
                stream.ResetPositionToStart();
                while (!stream.IsEof)
                {
                    char c = stream.ReadNextChar();
                    // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.                                                            
                    if (Char.IsLetter(c) && Char.IsLetter(nc) && (char.ToLower(c) == char.ToLower(nc)))
                    {
                        string p = c.ToString() + nc.ToString();
                        int i = r.FindIndex(x => x.Letter.Equals(p, StringComparison.OrdinalIgnoreCase));
                        if (i != -1)
                        {
                            r[i] = IncStatistic(r[i]);
                        } else
                        {
                            r.Add(new LetterStats() { Letter = p, Count = 1 });
                        }
                    }

                    // Сохраняем предыдущий символ
                    nc = c;
                }
            }

            return r;            
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(List<LetterStats> letters, CharType charType)
        {
            // TODO : Удалить статистику по запрошенному типу букв.
            // Т.к. у нас пары одинаковых символов, то достаточно взять первый символ в проверке для лямбды
            switch (charType)
            {
                // Удаляем согласные
                case CharType.Consonants:
                    letters.RemoveAll(x => !CharVowel.VOWEL.Contains(x.Letter[0]));
                    break;
                // Удаляем гласные
                case CharType.Vowel:                    
                    letters.RemoveAll(x => CharVowel.VOWEL.Contains(x.Letter[0]));
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
            List<LetterStats> a =  letters.ToList();
            a.Sort((x, y) => x.Letter.CompareTo(y.Letter));
            a.Add(new LetterStats() { Letter = "ИТОГО", Count = a.Count()});
            a.ForEach(x => Console.WriteLine($"{x.Letter} : {x.Count}"));             
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static LetterStats IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
            return letterStats;
        }


    }
}
