using System;
using System.Linq;
using System.Collections.Generic;
using TestTask.Helpers;
using TestTask.Models;
using TestTask.Interfaces;

namespace TestTask
{
    public class Program
    {
        #region Private Methods

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
            //string firstTestFilePath = args[0];
            //string secondTestFilePath = args[1];

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
                RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                //PrintStatistic(singleLetterStats);
            }

            //Выполнение второй задачи приложения.
            using (IReadOnlyStream doubleCharInputStream = GetInputStream(firstTestFilePath))
            {
                IList<LetterStats> doubleLetterStats = GetSymbolStatistic(doubleCharInputStream, FillDoubleLetterStats);
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
        /// Выполняет анализ текстового потока данных и возвращает статистику составленную в соответствии с некоторой, переданной в метод, логикой.
        /// </summary>
        /// <param name="symbolStream">Ана0лизируемый поток данных.</param>
        /// <param name="CreateStatisticCallback">Метод реализующий логику анализа данных.</param>
        /// <returns>Сведенная статистика.</returns>
        private static List<LetterStats> GetSymbolStatistic(IReadOnlyStream symbolStream, Action<IReadOnlyStream, IList<LetterStats>> CreateStatisticCallback)
        {             // ^ Заменил возвращаемый тип на более конкретный, так как согласно одного из дополнений к принципу Барбары Лисков -
                      //   должна быть поддержана контрвариантность возвращаемых значений.

            symbolStream.ResetPositionToStart();

            var resultStatistic = new List<LetterStats>();

            CreateStatisticCallback?.Invoke(symbolStream, resultStatistic);

            return resultStatistic;
        }

        /// <summary>
        /// Анализирует входящий символ, и добавляет его к статистике, если символ является буквой.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="symbolStream">Входящий символ для анализа.</param>
        /// <param name="statCollection">Коллекция статистик.</param>
        private static void FillSingleLetterStats(IReadOnlyStream symbolStream, IList<LetterStats> statCollection)
        {
            // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.

            while (!symbolStream.IsEndOfFile)
            {
                char symbol = symbolStream.ReadNextChar();

                if (Char.IsLetter(symbol))
                {
                    var availableSymbol = statCollection.FirstOrDefault(stat => stat.Letter == symbol.ToString());
                    if (availableSymbol != null)
                    {
                        IncStatistic(availableSymbol);
                    }
                    else
                    {
                        var newStats = new LetterStats(symbol.ToString());
                        statCollection.Add(newStats);
                    }
                }
            }
        }

        /// <summary>
        /// Анализирует входящий символ, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="symbolStream">Стрим для считывания символов для последующего анализа</param>
        /// <param name="statCollection">Коллекция статистик.</param>
        private static void FillDoubleLetterStats(IReadOnlyStream symbolStream, IList<LetterStats> statCollection)
        {
            // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.

            char previousSymbol = char.MinValue;

            while (!symbolStream.IsEndOfFile)
            {
                char currentSymbol = symbolStream.ReadNextChar();

                if (Char.IsLetter(currentSymbol))
                {
                    char upperCurrentSymbol = Char.ToUpper(currentSymbol);

                    if (upperCurrentSymbol != previousSymbol)
                    {
                        previousSymbol = upperCurrentSymbol;
                        continue;
                    }

                    string doubleLetter = $"{previousSymbol}{upperCurrentSymbol}";

                    var availableSymbol = statCollection.FirstOrDefault(stat => stat.Letter == doubleLetter);

                    if (availableSymbol != null)
                    {
                        IncStatistic(availableSymbol);
                    }
                    else
                    {
                        var newStats = new LetterStats(doubleLetter);
                        statCollection.Add(newStats);
                    }
                }
                else
                {
                    previousSymbol = char.MinValue;
                }
            }
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной статистике.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
        }

        /// <summary>
        /// Метод перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
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



        #endregion Private Methods

        #region Блоки исходной реализации

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

        #endregion Блоки исходной реализации.
    }
}
