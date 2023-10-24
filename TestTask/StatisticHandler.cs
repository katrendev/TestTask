using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    internal class StatisticHandler
    {
        private const string _vowelLetters = "aeiouyаеёийоуыэюя";
        private List<LetterStats> _LetterStats;

        internal IList<LetterStats> GetLetterStatistic(string filePath, bool isDoubleLetterStats = false, bool ignoreCase = false)
        {
            IList<LetterStats> result = new List<LetterStats>();
            char lastLetter = default;

            using (var stream = new ReadOnlyStream(filePath))
            {
                stream.ResetPositionToStart();
                while (!stream.IsEndOfStream)
                {
                    char letter = stream.ReadNextChar();
                    if (char.IsLetter(letter))
                    {
                        if (ignoreCase)
                        {
                            letter = char.ToUpper(letter);
                        }

                        if (isDoubleLetterStats && lastLetter != letter)
                        {
                            lastLetter = letter;
                            continue;
                        }

                        var stringLetter = isDoubleLetterStats ? lastLetter + letter.ToString() : letter.ToString();
                        var containedValue = result.FirstOrDefault(r => r.Letter == stringLetter);

                        if (containedValue != null)
                        {
                            containedValue.Count++;
                        }
                        else
                        {
                            result.Add(new LetterStats()
                            {
                                Letter = stringLetter,
                                Count = 1,
                            });
                        }

                        lastLetter = default;
                    }                    
                }
            }
            return result;
        }

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
