using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestTask.Data.English;
using TestTask.Enums;
using TestTask.Helpers;
using TestTask.Models;
using TestTask.Streams;
using TestTask.Streams.Interfaces;

namespace TestTask.Services
{
    internal sealed class StatisticService
    {
        #region Private Fields

        private string _fileToAnalyze;

        /// <summary>
        /// Статистика по буквам в удобном для модификации формате.
        /// </summary>
        private Lazy<IDictionary<string, int>> _lazyStatistic = new Lazy<IDictionary<string, int>>();

        /// <summary>
        /// Возвращает статистику анализа.
        /// </summary>
        /// <returns>Статистика содержимого файла.</returns>
        /// TODO реализовать хранение готовой статистики.
        public IEnumerable<LetterStats> GetResult => CreateStatistic(_lazyStatistic.Value);

        #endregion Private Fields

        #region Public Methods

        public void SetFilePath(string fileToAnalyze)
        {
            _fileToAnalyze = fileToAnalyze;
        }

        /// <summary>
        /// Запускает процесс анализа файла.
        /// </summary>
        public void StartAnalyzing()
        {
            IEnumerable<LetterStats> lettersStats;

            //TODO Проверить корректность путь до файла.
            using (IReadOnlyStream inputStream1 = GetInputStream(_fileToAnalyze))
            {
                lettersStats = FillLetterStats(inputStream1);
            }

            lettersStats = RemoveCharStatsByType(lettersStats, CharType.Consonants);

            PrintStatistic(lettersStats);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Создает статистику из <see cref="Dictionary{T, int}"/>
        /// </summary>
        /// <typeparam name="T">Тип ключа словаря.</typeparam>
        /// <param name="stats">Статистика в виде словаря.</param>
        /// <returns>Статистика в виде <see cref="List{LetterStats}"/>.</returns>
        private static IEnumerable<LetterStats> CreateStatistic(IDictionary<string, int> stats)
        {
            foreach (var stat in stats)
            {
                yield return new LetterStats(stat.Key, stat.Value);
            }
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IEnumerable<LetterStats> FillLetterStats(IReadOnlyStream stream, int sumOfLettersForAnalysis = 1)
        {
            IDictionary<string, int> statistic = new Dictionary<string, int>();

            stream.ResetPositionToStart();

            StringBuilder analyzingLetters = new StringBuilder();

            for (int i = 1; i < sumOfLettersForAnalysis; i++)
            {
                if (!stream.IsEof)
                {
                    analyzingLetters.Append(stream.ReadNextChar());
                }
            }

            while (!stream.IsEof)
            {
                ///TODO проверка на конец файла.
                analyzingLetters.Append(stream.ReadNextChar());

                foreach (var a in analyzingLetters.ToString())
                {
                    
                }


                bool isDoubledLetters = string.Equals(analyzingLetters[0].ToString(), analyzingLetters[1].ToString(), StringComparison.OrdinalIgnoreCase);

                if (isDoubledLetters)
                {
                    string doubledLettersString = analyzingLetters.ToString();

                    //TODO вынести в отдельный метод.
                    if (statistic.ContainsKey(doubledLettersString))
                    {
                        statistic[doubledLettersString]++;
                    }
                    else
                    {
                        statistic.Add(doubledLettersString, 1);
                    }
                }

                analyzingLetters.Remove(0, 1);
            }

            return CreateStatistic(statistic);
        }

        private bool CompareStringItself(string stringToCompare, bool isIgnoreCase)
        {
            if (isIgnoreCase)
            {
                return IsEqualsIgoreCase();
            }
            else
            {

            }

            for (int i = 0; i < 0; i++)
            {
                for (int j = 0; j < 0; j++)
                {

                }
            }

            bool IsEqualsIgoreCase()
            {
                string lowerString = stringToCompare.ToLower();

                bool isEquals = true;

                foreach (char lowerChar in lowerString)
                {
                    if (!lowerChar.Equals(lowerString[0]))
                    {
                        isEquals = false; 
                    };
                }

                return isEquals;
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
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(IEnumerable<LetterStats> stats, bool isSortingNecessary = true)
        {
            if (isSortingNecessary)
            {
                stats = stats.OrderBy(stat => stat.Letter);
            }

            foreach (var stat in stats)
            {
                ConsoleHelper.WriteLine("{0} : {1}", stat.Letter, stat.Count);
            }

            ConsoleHelper.WriteLine("Letters count : {0 }", stats.Count());
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static IEnumerable<LetterStats> RemoveCharStatsByType(IEnumerable<LetterStats> statistic, CharType charType)
        {
            string charsFilter = null;

            switch (charType)
            {
                case CharType.Consonants:
                    charsFilter = ListOfCharsTypes.ConsonantsChars;
                    break;

                case CharType.Vowel:
                    charsFilter = ListOfCharsTypes.VovelChars;
                    break;

                default:
                    charsFilter = ListOfCharsTypes.ConsonantsChars;
                    break;
            }

            foreach (var letterStat in statistic)
            {
                char letterToCompare = letterStat.Letter.ToString()[0];

                if (charsFilter.Contains(letterToCompare))
                {
                    yield return letterStat;
                }
            }
        }

        #endregion Private Methods
    }
}