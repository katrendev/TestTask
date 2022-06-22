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
        #region Private Fields

        /// <summary>
        /// Дефолтное значение <see cref="_compareCharsCount"/>.
        /// </summary>
        private const int DEFAULT_CHARS_COMPARE_COUNT = 1;

        /// <summary>
        /// Количество символов для сравнения.
        /// </summary>
        private BuilderProperty<int> _compareCharsCount = new BuilderProperty<int>()
        {
            Value = DEFAULT_CHARS_COMPARE_COUNT
        };

        /// <summary>
        /// Путь до файла, который необходимо анализировать.
        /// </summary>
        private BuilderProperty<string> _fileToAnalyze = new BuilderProperty<string>();

        /// <summary>
        /// Необходимо ли игнорировать регистр при сравнении строк.
        /// </summary>
        private BuilderProperty<bool> _isIgnoreCaseRequire = new BuilderProperty<bool>();

        /// <summary>
        /// Cтатистика анализа файла.
        /// </summary>
        private IEnumerable<LetterStats> _result;

        /// <summary>
        /// Статистика по буквам в удобном для модификации формате.
        /// </summary>
        private IDictionary<string, int> _statistic = new Dictionary<string, int>();

        /// <inheritdoc cref="_result"/>
        public IEnumerable<LetterStats> Result => _result;

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
        /// </summary>
        public void StartAnalyzing()
        {
            if (_fileToAnalyze.IsValueSetted)
            {
                //TODO Проверить корректность путь до файла.
                using (IReadOnlyStream inputStream1 = GetInputStream(_fileToAnalyze.Value))
                {
                    _result = GetLetterStats(inputStream1, _compareCharsCount.Value);
                }

                _result = CreateStatistic(_statistic);

                if (_charsTypeToResult.IsValueSetted)
                {
                    _result = RemoveCharStatsByType(_result, _charsTypeToResult.Value);
                }
            }

            InvokeAnalyzingCompleted();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Уведомляет о завершении процесса сканирования файла.
        /// </summary>
        public event EventHandler<AnalyzingCompletedEventArgs> AnalyzingCompleted;

        /// <summary>
        /// Создает статистику из <see cref="Dictionary{T, int}"/>
        /// </summary>
        /// <typeparam name="T">Тип ключа словаря.</typeparam>
        /// <param name="stats">Статистика в виде словаря.</param>
        /// <returns>Статистика в виде <see cref="List{LetterStats}"/>.</returns>
        private IEnumerable<LetterStats> CreateStatistic(IDictionary<string, int> stats)
        {
            foreach (var stat in stats)
            {
                yield return new LetterStats(stat.Key, stat.Value);
            }
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
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private IEnumerable<LetterStats> GetLetterStats(IReadOnlyStream stream, int compareCharsCount)
        {
            StringBuilder analyzingLetters = new StringBuilder();

            stream.ResetPositionToStart();

            //Создание начальной строки для сравнения.
            //Если необходимо сравнивать по 1 символу, то заполнения начальной строки не будет.
            for (int i = 1; i < compareCharsCount; i++)
            {
                if (!stream.IsEof)
                {
                    analyzingLetters.Append(stream.ReadNextChar());
                }
            }

            while (!stream.IsEof)
            {
                analyzingLetters.Append(stream.ReadNextChar());

                bool isPartsEquals = IsStringPartsEquals(analyzingLetters.ToString(), _isIgnoreCaseRequire.Value);

                if (isPartsEquals)
                {
                    string entryToStat = analyzingLetters.ToString();

                    IncStatistic(entryToStat);
                }

                analyzingLetters.Remove(0, 1);
            }

            return CreateStatistic(_statistic);
        }

        /// <summary>
        /// Записывает найденную строку в статистику.
        /// </summary>
        /// <param name="entryToStatistic">Строка для записи.</param>
        private void IncStatistic(string entryToStatistic)
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
        /// TODO создать кастомный аргумент.
        private void InvokeAnalyzingCompleted()
        {
            AnalyzingCompletedEventArgs args = new AnalyzingCompletedEventArgs(Result);
            AnalyzingCompleted?.Invoke(this, args);
        }

        /// <summary>
        /// Сравнивает части строки друг с другом.
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

            bool isEquals = true;

            foreach (char lowerChar in stringToCompare)
            {
                if (!lowerChar.Equals(stringToCompare[0]))
                {
                    isEquals = false;
                };
            }

            return isEquals;
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private IEnumerable<LetterStats> RemoveCharStatsByType(IEnumerable<LetterStats> stats, CharType charType)
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

            foreach (var letterStat in stats)
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