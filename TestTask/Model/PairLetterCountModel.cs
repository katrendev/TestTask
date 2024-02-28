using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTask.Model
{
    internal class PairLetterCountModel : ILetterCountModel
    {
        IReadOnlyStream inputStream;

        public void ReadStream(string filePath)
        {
            inputStream = new ReadOnlyStream(filePath);
        }

        /// <summary>
        ///  Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        ///  Аналог прошлой функции статическкого метода FillDoubleLetterStats(inputStream2).
        /// </summary>
        /// /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        public IList<LetterStats> GetLetterStats()
        {
            inputStream.ResetPositionToStart();
            var letterStats = new List<LetterStats>();

            while (!inputStream.IsEof)
            {
                char currentChar = inputStream.ReadNextChar();

                if (!inputStream.IsEof)
                {

                    if (!Char.IsWhiteSpace(currentChar))
                    {
                        char nextChar = inputStream.ReadNextChar();

                        if (!Char.IsWhiteSpace(nextChar))
                        {
                            string checkedSting = (nextChar.ToString() + currentChar).ToUpper();

                            if (letterStats.Count(x => x.Letter.ToUpper().Equals(checkedSting)) != 0)
                            {
                                IncStatistic(letterStats.First(x => x.Letter.ToUpper().Equals(checkedSting)));
                            }
                            else if (nextChar.Equals(currentChar))
                            {
                                letterStats.Add(new LetterStats()
                                {
                                    Letter = checkedSting,
                                    Count = 1
                                });
                            }

                            currentChar = nextChar;
                        }
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
