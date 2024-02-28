using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTask.Model
{
    public class SingleLetterCountModel : ILetterCountModel
    {
        IReadOnlyStream inputStream;

        public void ReadStream(string filePath)
        {
            inputStream = new ReadOnlyStream(filePath);
        }

        //аналог статического метода FillSingleLetterStats(inputStream1);
        public IList<LetterStats> GetLetterStats()
        {
            var letterStats = new List<LetterStats>();

            inputStream.ResetPositionToStart();

            while (!inputStream.IsEof)
            {
                char currentChar = inputStream.ReadNextChar();

                if (!Char.IsWhiteSpace(currentChar))
                {
                    if (letterStats.Count(x => x.Letter == currentChar.ToString()) != 0)
                    {
                        IncStatistic(letterStats.First(x => x.Letter == currentChar.ToString()));
                    }
                    else
                    {
                        letterStats.Add(new LetterStats()
                        {
                            Letter = currentChar.ToString(),
                            Count = 1
                        });
                    }
                }
            }

            inputStream.FileClose();

            return letterStats;
        }

        public void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            char[] vowel = new char[] { 'а', 'е', 'ё', 'и', 'о', 'у', 'ы', 'э', 'ю', 'я', 'a', 'e', 'i', 'o', 'u', 'y' };

            for (int i = 0; i < letters.Count; i++)
            {
                switch (charType)
                {
                    case CharType.Consonants:

                        if (!vowel.Contains(letters[i].Letter[0]))
                        {
                            letters.Remove(letters[i]);
                        }

                        break;

                    case CharType.Vowel:
                        if (vowel.Contains(letters[i].Letter[0]))
                        {
                            letters.Remove(letters[i]);
                        }

                        break;

                }
            }
        }

        private static void IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
        }
    }
}
