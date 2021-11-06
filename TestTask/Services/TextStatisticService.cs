using System;
using System.Collections.Generic;
using System.Linq;
using TestTask.Helpers;
using TestTask.Interfaces;
using TestTask.Models;

namespace TestTask.Services
{
    /// <summary>
    /// Представляет набор методов для анализа текстового файла.
    /// </summary>
    internal class TextStatisticService
    {
        #region Public Methods

        /// <summary>
        /// Возвращает статистику вхождений одиночных символов.
        /// </summary>
        /// <param name="symbolStream">Входящий поток.</param>
        /// <returns>Коллекция статистик.</returns>
        public List<LetterStats> GetDoubleSymbolStatistic(IReadOnlyStream symbolStream)
        {
            return GetSymbolStatistic(symbolStream, FillDoubleLetterStats);
        }

        /// <summary>
        /// Возвращает статистику вхождений парных символов.
        /// </summary>
        /// <param name="symbolStream">Входящий поток.</param>
        /// <returns>Коллекция статистик.</returns>
        public List<LetterStats> GetSingleSymbolStatistic(IReadOnlyStream symbolStream)
        {
            return GetSymbolStatistic(symbolStream, FillSingleLetterStats);
        }

        /// <summary>
        /// Метод возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        public IReadOnlyStream GetInputStream(string fileFullPath)
        {
            return new ReadOnlyStream(fileFullPath);
        }

        /// <summary>
        /// Метод выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="stats">Коллекция со статистикой</param>
        public void PrintStatistic(IEnumerable<LetterStats> stats, int documentNumber)
        {
            // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
            Console.WriteLine($"-- Результ анализа документа № {documentNumber} --");

            var sortStats = stats.OrderBy(stat => stat.Letter);
            foreach (var stat in sortStats)
            {
                Console.WriteLine($"{stat.Letter} : {stat.Count}");
            }

            Console.WriteLine($"ИТОГО: {stats.Count()}\n");
        }

        /// <summary>
        /// Метод перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        public void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            // TODO : Удалить статистику по запрошенному типу букв.
            Action<string> removeLogic = charTypeCollection =>
            {
                if (letters is List<LetterStats> lettersList)
                {
                    foreach (var letter in charTypeCollection)
                    {
                        lettersList.RemoveAll(stat => stat.Letter.ToUpper().FirstOrDefault() == letter);
                    }
                }
            };

            switch (charType)
            {
                case CharType.Consonants:
                    removeLogic(CharTypeHelper.RUS_EN_CONSONANTS);
                    break;
                case CharType.Vowel:
                    removeLogic(CharTypeHelper.RUS_EN_VOWELS);
                    break;
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной статистике.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
        }

        /// <summary>
        /// Анализирует входящий символ, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="symbolStream">Стрим для считывания символов для последующего анализа</param>
        /// <param name="statCollection">Коллекция статистик.</param>
        private void FillDoubleLetterStats(IReadOnlyStream symbolStream, IList<LetterStats> statCollection)
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
        /// Анализирует входящий символ, и добавляет его к статистике, если символ является буквой.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="symbolStream">Входящий символ для анализа.</param>
        /// <param name="statCollection">Коллекция статистик.</param>
        private void FillSingleLetterStats(IReadOnlyStream symbolStream, IList<LetterStats> statCollection)
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
        /// Выполняет анализ текстового потока данных и возвращает статистику составленную в соответствии с некоторой, переданной в метод, логикой.
        /// </summary>
        /// <param name="symbolStream">Ана0лизируемый поток данных.</param>
        /// <param name="CreateStatisticCallback">Метод реализующий логику анализа данных.</param>
        /// <returns>Сведенная статистика.</returns>
        private List<LetterStats> GetSymbolStatistic(IReadOnlyStream symbolStream, Action<IReadOnlyStream, IList<LetterStats>> CreateStatisticCallback)
        {             // ^ Заменил возвращаемый тип на более конкретный, так как согласно одного из дополнений к принципу Барбары Лисков -
                      //   должна быть поддержана контрвариантность возвращаемых значений.

            symbolStream.ResetPositionToStart();

            var resultStatistic = new List<LetterStats>();

            CreateStatisticCallback?.Invoke(symbolStream, resultStatistic);

            return resultStatistic;
        }

        #endregion Private Methods
    }
}
