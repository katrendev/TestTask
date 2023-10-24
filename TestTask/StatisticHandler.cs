using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    internal class StatisticHandler
    {
        private const string _vowelLetters = "aeiouyаеёийоуыэюя";
        private List<LetterStats> _LetterStats;

        /// <summary>
        /// Получение статистики букв
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <param name="isDoubleLetterStats">Искать удвоенные буквы</param>
        /// <param name="ignoreCase">Игнорировать регистр</param>
        /// <returns></returns>
        internal IList<LetterStats> GetLetterStatistic(string filePath, bool isDoubleLetterStats = false, bool ignoreCase = false)
        {
            IList<LetterStats> result = new List<LetterStats>();
            char lastCharacter = default;

            using (var stream = new ReadOnlyStream(filePath))
            {
                stream.ResetPositionToStart();
                while (!stream.IsEndOfStream)
                {
                    char character = ignoreCase ? char.ToUpper(stream.ReadNextChar()) : stream.ReadNextChar();

                    if (char.IsLetter(character))
                    {
                        if (isDoubleLetterStats && lastCharacter != character)
                        {
                            lastCharacter = character;
                            continue;
                        }

                        var letter = isDoubleLetterStats ? lastCharacter + character.ToString() : character.ToString();
                        var containedValue = result.FirstOrDefault(r => r.Letter == letter);

                        if (containedValue != null)
                        {
                            containedValue.Count++;
                        }
                        else
                        {
                            result.Add(new LetterStats()
                            {
                                Letter = letter,
                                Count = 1,
                            });
                        }

                        lastCharacter = default;
                    }
                    else
                    {
                        lastCharacter = character;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Удалить буквы определённого типа
        /// </summary>
        /// <param name="letters">Список букв</param>
        /// <param name="charType">Удаляемый тип</param>
        /// <returns></returns>
        internal IList<LetterStats> RemoveCharTypes(IList<LetterStats> letters, CharType charType)
        {
            for (int i = 0; i < letters.Count; i++)
            {
                if (charType == CharType.Vowel && IsVowel(letters[i].Letter[0]) ||
                    charType == CharType.Consonants && !IsVowel(letters[i].Letter[0]))
                {
                    letters.Remove(letters[i]);
                    i--;
                }
            }

            return letters;
        }

        private bool IsVowel(char letter) => _vowelLetters.Contains(char.ToLower(letter));
    }
}
