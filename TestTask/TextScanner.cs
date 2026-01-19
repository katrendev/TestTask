using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    /// <summary>
    /// Обработчик текстовых файлов с поиском в них нужных символов
    /// </summary>
    public class TextScanner
    {
        /// <summary>
        /// Все гласные буквы (в нижнем регистре)
        /// </summary>
        public const string VOWEL_LETTERS = "аеёиоуыэюяaeiouy";
        /// <summary>
        /// Все согласные буквы (в нижнем регистре)
        /// </summary>
        public const string CONSONANTS_LETTERS = "бвгджзйклмнпрстфхцчшщbcdfghjklmnpqrstvwxz";
        /// <summary>
        /// Все буквы (в нижнем регистре)
        /// </summary>
        public const string ALL_LETTERS = VOWEL_LETTERS + CONSONANTS_LETTERS + "ъь";

        private readonly string _SourceFilePath;

        /// <summary>
        /// Регистрозависимость поиска
        /// </summary>
        public bool CaseSensitive { get; set; }

        /// <summary>
        /// Вид поиска (одиночные или парные буквы)
        /// </summary>
        public SearchKind SearchKind { get; set; }

        /// <summary>
        /// Конструктор класса <see cref="TextScanner"/>
        /// </summary>
        /// <param name="sourceFilePath">путь к текстовому файлу</param>
        public TextScanner(string sourceFilePath)
        {
            this._SourceFilePath = sourceFilePath;
        }

        /// <summary>
        /// ВЫполняет поиск букв в текстовом файле
        /// </summary>
        /// <returns>результат поиска</returns>
        public SearchResult Search()
        {
            string allLetters = this.CaseSensitive ? ALL_LETTERS.ToUpper() + ALL_LETTERS : ALL_LETTERS;
            var foundChars = new Dictionary<char, int>();
            using (var stream = new ReadOnlyStream(this._SourceFilePath))
            {
                stream.ResetPositionToStart();
                char? previousChar = null;
                while (!stream.IsEof)
                {
                    bool isCharMatched = false;
                    char currentCharOriginal = stream.ReadNextChar();
                    char currentChar = this.CaseSensitive ? currentCharOriginal : char.ToLower(currentCharOriginal);
                    switch (this.SearchKind)
                    {
                        case SearchKind.SingleLetters:
                        {
                            isCharMatched = allLetters.Contains(currentChar);
                            break;
                        }
                        case SearchKind.DoubleLetters:
                        {
                            isCharMatched = previousChar != null && previousChar.Equals(currentChar) && allLetters.Contains(currentChar);
                            break;
                        }
                    }

                    if (isCharMatched)
                    {
                        if (!foundChars.ContainsKey(currentChar))
                        {
                            foundChars.Add(currentChar, 1);
                        }
                        else
                        {
                            foundChars[currentChar]++;
                        }
                    }

                    previousChar = currentChar;
                }
            }
            return new SearchResult(this._SourceFilePath, foundChars, this.CaseSensitive, this.SearchKind);
        }
    }
}
