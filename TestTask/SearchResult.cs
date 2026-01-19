using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestTask
{
    /// <summary>
    /// Результат со статистикой поиска нужных символов в текстовом файле
    /// </summary>
    public class SearchResult
    {
        private readonly string _SourceFilePath;
        private readonly IDictionary<char, int> _FoundChars;

        /// <summary>
        /// Показывает, являлся ли поиск регистрозависимым
        /// </summary>
        public bool CaseSensitive { get; }

        /// <summary>
        /// Показывает, какой вид поиска применялся
        /// </summary>
        public SearchKind SearchKind { get; }

        /// <summary>
        /// Cтатистика по найденным символам, представленная с использованием типа <see cref="LetterStats"/>
        /// </summary>
        public IEnumerable<LetterStats> LetterStatsCollection
        {
            get { return this._FoundChars.OrderBy(fc => fc.Key).Select(foundChar => new LetterStats() {Letter = foundChar.Key, Count = foundChar.Value}); }
        }

        /// <summary>
        /// Конструктор класса <see cref="SearchResult"/>
        /// </summary>
        /// <param name="sourceFilePath">путь к исходному файлу</param>
        /// <param name="foundChars">найденные символы</param>
        /// <param name="caseSensitive">признак регистрозависимости поиска</param>
        /// <param name="searchKind">вид поиска</param>
        public SearchResult(string sourceFilePath, IDictionary<char, int> foundChars, bool caseSensitive, SearchKind searchKind)
        {
            this._SourceFilePath = sourceFilePath;
            this._FoundChars = foundChars;
            this.CaseSensitive = caseSensitive;
            this.SearchKind = searchKind;
        }

        /// <summary>
        /// Исключает из результата поиска статистику по заданному типу букв
        /// </summary>
        /// <param name="charType">тип букв, статистику по которым следует исключить</param>
        public void ExcludeLettersByCharType(CharType charType)
        {
            char[] excludingChars = new char[] { };
            switch (charType)
            {
                case CharType.Vowel:
                {
                    excludingChars = (this.CaseSensitive ? TextScanner.VOWEL_LETTERS.ToUpper() + TextScanner.VOWEL_LETTERS : TextScanner.VOWEL_LETTERS).ToCharArray();
                    break;
                }
                case CharType.Consonants:
                {
                    excludingChars = (this.CaseSensitive ? TextScanner.CONSONANTS_LETTERS.ToUpper() + TextScanner.CONSONANTS_LETTERS : TextScanner.CONSONANTS_LETTERS).ToCharArray();
                    break;
                }
            }

            foreach (char excludingChar in excludingChars)
            {
                this._FoundChars.Remove(excludingChar);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Файл \"{this._SourceFilePath}\" ({(this.CaseSensitive ? "регистрозависимо" : "регистроНЕзависимо")}, {this.SearchKind.ToFriendlyString()})");
            foreach (var foundChar in this._FoundChars.OrderBy(fc => fc.Key))
            {
                stringBuilder.AppendLine($"{foundChar.Key} : {foundChar.Value}");
            }
            stringBuilder.AppendLine("ИТОГО: " + this._FoundChars.Count);
            return stringBuilder.ToString();
        }
    }
}