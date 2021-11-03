using System;
using System.Linq;
using System.Collections.Generic;

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
            //IReadOnlyStream inputStream1 = GetInputStream(args[0]);
            //IReadOnlyStream inputStream2 = GetInputStream(args[1]);

            //string firstTestFilePath = @"C:\Users\Alter\Desktop\B.txt";
            //string secondTestFilePath = @"C:\Users\Alter\Desktop\A.txt"; 

            string firstTestFilePath = @"C:\MyData\Soft\WinRar\ReadMe.rus.txt";
            string secondTestFilePath = @"C:\MyData\Soft\Git\LICENSE.txt";

            //try
            //{
            //Выполнение первой задачи приложения.
            using (IReadOnlyStream singleCharInputStream = GetInputStream(secondTestFilePath))
            {
                IList<LetterStats> singleLetterStats = GetSymbolStatistic(singleCharInputStream, FillSingleLetterStats);
                //RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                //PrintStatistic(singleLetterStats);
            }

            //Выполнение второй задачи приложения.
            using (IReadOnlyStream inputStream2 = GetInputStream(firstTestFilePath))
            {
                //IList<LetterStats> doubleLetterStats = GetSymbolStatistic(inputStream2);
                //RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);
                //PrintStatistic(doubleLetterStats);
            }
            //}
            //catch(DirectoryNotFoundException ex)
            //{
            //OverflowException когда -1
            //}

            // TODO : Необходимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
            Console.ReadKey();
        }

        /// <summary>
        /// Метод возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        private static IReadOnlyStream GetInputStream(string fileFullPath)
        {
            return new ReadOnlyStream(fileFullPath);
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
        }

        /// <summary>
        /// Выполняет анализ текстового потока данных и возвращает статистику составленную в соответствии с некоторой, переданной в метод, логикой.
        /// </summary>
        /// <param name="symbolStream">Анализируемый поток данных.</param>
        /// <param name="addToStatisticCallback">Метод реализующий логику анализа данных.</param>
        /// <returns>Сведенную статистику.</returns>
        private static List<LetterStats> GetSymbolStatistic(IReadOnlyStream symbolStream, Action<char, IList<LetterStats>> addToStatisticCallback)
        {             // ^ Заменил возвращаемый тип на более конкретный, так как согласно одного из дополнений принципа Барбары Лисков -
                      //   должна быть поддержана контрвариантность возвращаемых значений.

            symbolStream.ResetPositionToStart();

            var resultStatistic = new List<LetterStats>();

            while (!symbolStream.IsEndOfFile)
            {
                char c = symbolStream.ReadNextChar();
                addToStatisticCallback.Invoke(c, resultStatistic);
            }

            return resultStatistic;
        }

        /// <summary>
        /// Анализирует входящий символ, и добавляет его к статистике, если символ является буквой.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="symbol">Входящий символ для анализа.</param>
        /// <param name="statCollection">Коллекция статистик.</param>
        private static void FillSingleLetterStats(char symbol, IList<LetterStats> statCollection)
        {
            // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
            if (Char.IsLetter(symbol))
            {
                var availableSymbol = statCollection.FirstOrDefault(stat => stat.Letter == symbol);
                if (availableSymbol != null)
                {
                    IncStatistic(availableSymbol);
                }
                else
                {
                    var newStats = new LetterStats(symbol);
                    statCollection.Add(newStats);
                }
            }
        }

        /// <summary>
        /// Анализирует входящий символ, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillDoubleLetterStats(char symbol, IList<LetterStats> statCollection)
        {



            return null;
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
                    break;
                case CharType.Vowel:
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
            throw new NotImplementedException();
        }






















        // На всякий случай не стал удалять исходную реализацию.

        /// <summary>
        /// Метод считывающий из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        //private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        //{
        //    stream.ResetPositionToStart();

        //    while (!stream.IsEndOfFile)
        //    {
        //        char c = stream.ReadNextChar();
        //        // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
        //    }

        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        //private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        //{
        //    stream.ResetPositionToStart();
        //    while (!stream.IsEndOfFile)
        //    {
        //        char c = stream.ReadNextChar();
        //        // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.
        //    }

        //    //return ???;

        //    throw new NotImplementedException();
        //}
    }
}
