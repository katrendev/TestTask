using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    public static class LetterStatsFiller
    {
        //     Статистика регистрозависимая
        public static IEnumerable<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            var letterStatsDictionary = new Dictionary<char, LetterStats>();
            stream.ResetPositionToStart();

            while (!stream.IsEof)
            {
                var currentChar = stream.ReadNextChar();

                if (!char.IsLetter(currentChar))
                {
                    continue;
                }
                if (!letterStatsDictionary.ContainsKey(currentChar))
                {
                    letterStatsDictionary[currentChar] = new LetterStats(currentChar);
                }

                letterStatsDictionary[currentChar].IncCount();
            }

            return letterStatsDictionary.Values;
        }
        //     Статистика - не регистрозависимая!
        public static IEnumerable<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            var pairStatses = new Dictionary<char, LetterStats>();
            stream.ResetPositionToStart();

            var previousChar = '\0';

            while (!stream.IsEof)
            {
                var currentChar = stream.ReadNextChar();

                if (!char.IsLetter(currentChar))
                {
                    previousChar = '\0';
                    continue;
                }

                currentChar = char.ToLower(currentChar);

                if (previousChar != currentChar)
                {
                    previousChar = currentChar;
                    continue;
                }
                if (!pairStatses.ContainsKey(currentChar))
                {
                    pairStatses[currentChar] = new LetterStats(currentChar);
                }

                var letterStats = pairStatses[currentChar];
                letterStats.IncCount();
            }

            return pairStatses.Values;
        }
    }
}