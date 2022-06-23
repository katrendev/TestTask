using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestTask.Data.English;
using TestTask.Enums;
using TestTask.EventsArgs;
using TestTask.Models;
using TestTask.Streams;
using TestTask.Streams.Interfaces;

namespace TestTask.Services
{
    /// <summary>
    /// Сервис статистики.
    /// </summary>
    internal sealed class StatisticService
    {
        #region Public Constructors

        /// <summary>
        /// Инициализирует экземпляр <see cref="StatisticService"/>.
        /// </summary>
        public StatisticService()
        {
            _compareCharsCount = new BuilderProperty<int>()
            {
                Value = DEFAULT_CHARS_COMPARE_COUNT
            };

            _fileToAnalyze = new BuilderProperty<string>();
            _isIgnoreCaseRequire = new BuilderProperty<bool>();
            _statistic = new Dictionary<string, int>();
        }

        #endregion Public Constructors

        #region Private Fields

        /// <summary>
        /// Дефолтное значение <see cref="_compareCharsCount"/>.
        /// </summary>
        private const int DEFAULT_CHARS_COMPARE_COUNT = 1;

        /// <summary>
        /// Количество символов для сравнения.
        /// </summary>
        private BuilderProperty<int> _compareCharsCount;

        /// <summary>
        /// Путь до файла, который необходимо анализировать.
        /// </summary>
        private BuilderProperty<string> _fileToAnalyze;

        /// <summary>
        /// Необходимо ли игнорировать регистр при сравнении строк.
        /// </summary>
        private BuilderProperty<bool> _isIgnoreCaseRequire;

        /// <summary>
        /// Cтатистика анализа файла.
        /// </summary>
        private IEnumerable<EntryStats> _result;

        /// <summary>
        /// Статистика по буквам в удобном для модификации формате.
        /// </summary>
        private IDictionary<string, int> _statistic;

        /// <summary>
        /// Сбрасывает результаты анализа.
        /// </summary>
        public void ResetResult()
        {
            _result = null;

            _statistic.Clear();
        }

        /// <summary>
        /// Устанавливает дефолтные настройки.
        /// </summary>
        public void ResetSettings()
        {
            _compareCharsCount.Value = DEFAULT_CHARS_COMPARE_COUNT;

            _fileToAnalyze.Reset();

            _isIgnoreCaseRequire.Reset();
        }

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Выборка резутатов по типу букв.
        /// </summary>
        private BuilderProperty<CharType> _charsTypeToResult = new BuilderProperty<CharType>();

        /// <summary>
        /// Был ли задан флаг <see cref="_charsTypeToResult"/>.
        /// </summary>
        private BuilderProperty<bool> _isCharsTypeResultSetted = new BuilderProperty<bool>();

        /// <summary>
        /// Устанавливет отсев статистики по типу букв.
        /// </summary>
        /// <param name="charsToResult">Тип букв для статистики.</param>
        public StatisticService SetCharsTypeResulting(CharType charsToResult)
        {
            _charsTypeToResult.Value = charsToResult;
            _isCharsTypeResultSetted.Value = true;

            return this;
        }

        /// <summary>
        /// Устанавливает количество символов для сравнения. Дефолтное значение - 1;
        /// </summary>
        /// <param name="compareCharsCount">Количество символов для сравнения.</param>
        public StatisticService SetCompareCharsCount(int compareCharsCount)
        {
            _compareCharsCount.Value = compareCharsCount;

            return this;
        }

        /// <summary>
        /// Устанавливает путь до файла.
        /// </summary>
        /// <param name="fileToAnalyze">Путь до файла, который нужно анализировать.</param>
        public StatisticService SetFilePath(string fileToAnalyze)
        {
            _fileToAnalyze.Value = fileToAnalyze;

            return this;
        }

        /// <summary>
        /// Устаналивает - необходимо ли игнорировать регистр при сравнении строк.
        /// </summary>
        /// <param name="isIgnoreCaseRequire">необходимо ли игнорировать регистр при сравнении строк</param>
        public StatisticService SetIgnoreCaseRequire(bool isIgnoreCaseRequire)
        {
            _isIgnoreCaseRequire.Value = isIgnoreCaseRequire;

            return this;
        }

        /// <summary>
        /// Запускает процесс анализа файла.
        /// Метод завершает выполнение, когда произойдет полный анализ файла.
        /// </summary>
        public void StartAnalyzing()
        {
            if (_fileToAnalyze.IsValueSetted)
            {
                //TODO Проверить корректность путь до файла.
                using (IReadOnlyStream inputStream1 = GetInputStream(_fileToAnalyze.Value))
                {
                    _result = GetEntryStats(inputStream1, _compareCharsCount.Value);
                }

                _result = CreateStatistic(_statistic);

                if (_charsTypeToResult.IsValueSetted)
                {
                    _result = RemoveCharStatsByType(_result, _charsTypeToResult.Value);
                }
            }

            InvokeAnalyzingCompleted();

            ResetResult();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Уведомляет о завершении процесса сканирования файла.
        /// </summary>
        public event EventHandler<AnalyzingCompletedEventArgs> AnalyzingCompleted;

        /// <summary>
        /// Создает статистику в виде <see cref="IEnumerable{EntryStats}"/>.
        /// </summary>
        /// <param name="stats">Статистика в виде <see cref="IDictionary{string, int}"/>.</param>
        /// <returns>Статистика по вхождениям строк в файле.</returns>
        private IEnumerable<EntryStats> CreateStatistic(IDictionary<string, int> stats)
        {
            foreach (var stat in stats)
            {
                yield return new EntryStats(stat.Key, stat.Value);
            }
        }

        /// <summary>
        /// Возвращает статистику по символам.
        /// </summary>
        /// <param name="stream">Поток для считывания символов для последующего анализа.</param>
        /// <param name="compareCharsCount">Количество символов для сравнения.</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из потока.</returns>
        private IEnumerable<EntryStats> GetEntryStats(IReadOnlyStream stream, int compareCharsCount)
        {
            StringBuilder analyzingEntrys = new StringBuilder();

            stream.ResetPositionToStart();

            //Создание начальной строки для сравнения.
            //Если необходимо сравнивать по 1 символу, то заполнения начальной строки не будет.
            for (int i = 1; i < compareCharsCount; i++)
            {
                if (!stream.IsEof)
                {
                    analyzingEntrys.Append(stream.ReadNextChar());
                }
            }

            while (!stream.IsEof)
            {
                analyzingEntrys.Append(stream.ReadNextChar());
                string totalEntries = analyzingEntrys.ToString();

                bool isPartsEquals = IsStringPartsEquals(totalEntries, _isIgnoreCaseRequire.Value);

                if (isPartsEquals)
                {
                    IncStatistic(ref totalEntries);
                }

                analyzingEntrys.Remove(0, 1);
            }

            return CreateStatistic(_statistic);
        }

        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        private IReadOnlyStream GetInputStream(string fileFullPath)
        {
            return new ReadOnlyStream(fileFullPath);
        }

        /// <summary>
        /// Записывает найденную строку в статистику.
        /// </summary>
        /// <param name="entryToStatistic">Строка для записи.</param>
        private void IncStatistic(ref string entryToStatistic)
        {
            if (_statistic.ContainsKey(entryToStatistic))
            {
                _statistic[entryToStatistic]++;
            }
            else
            {
                _statistic.Add(entryToStatistic, 1);
            }
        }

        /// <summary>
        /// Вызывает событие <see cref="AnalyzingCompleted"/>.
        /// </summary>
        private void InvokeAnalyzingCompleted()
        {
            AnalyzingCompletedEventArgs args = new AnalyzingCompletedEventArgs(_result);
            AnalyzingCompleted?.Invoke(this, args);
        }

        /// <summary>
        /// Сравнивает символы строки друг с другом.
        /// </summary>
        /// <param name="stringToCompare">Строка, которую необходимо анализировать.</param>
        /// <param name="isIgnoreCase">Необходимо ли игнорировать регистр знаков.</param>
        /// <returns>Равны ли составляющие строки друг другу.</returns>
        private bool IsStringPartsEquals(string stringToCompare, bool isIgnoreCase)
        {
            if (isIgnoreCase)
            {
                stringToCompare = stringToCompare.ToLower();
            }

            foreach (char lowerChar in stringToCompare)
            {
                if (!lowerChar.Equals(stringToCompare[0]))
                {
                    return false;
                };
            }

            return true;
        }

        /// <summary>
        /// Отсеивает статистику по выбранным буквам (гласные/согласные/все).
        /// </summary>
        /// <param name="stats">Статистика по записям.</param>
        /// <param name="charType">Тип букв для отсева.</param>
        /// <returns>Отсеянная статистика</returns>
        private IEnumerable<EntryStats> RemoveCharStatsByType(IEnumerable<EntryStats> stats, CharType charType)
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
                    charsFilter = ListOfCharsTypes.All;
                    break;
            }

            foreach (var entryStat in stats)
            {
                char entryToCompare = entryStat.Entry.ToString()[0];

                if (charsFilter.Contains(entryToCompare))
                {
                    yield return entryStat;
                }
            }
        }

        #endregion Private Methods
    }
}