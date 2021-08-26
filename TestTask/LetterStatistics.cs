using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// ReSharper disable All

namespace TestTask
{
    public static class LetterStatistics
    {
        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        public static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            if (stream == Stream.Null) return null;

            IDictionary<char, LetterStats> letterStatsMap = new Dictionary<char, LetterStats>();
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c = (char) 0;
                try
                {
                    c = stream.ReadNextChar();
                }
                catch (EndOfStreamException e)
                {
                    Console.WriteLine(e);
                    break;
                }

                if (letterStatsMap.ContainsKey(c))
                {
                    LetterStats tmp = letterStatsMap[c];
                    IncStatistic(ref tmp);
                    letterStatsMap[c] = tmp;
                }
                else if (Char.IsLetter(c))
                {
                    letterStatsMap.Add(c, new LetterStats()
                    {
                        Count = 1,
                        Letter = c.ToString()
                    });
                }
            }

            return letterStatsMap.Select(item => item.Value).ToList();
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        public static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            if (stream == Stream.Null) return null;

            IDictionary<char, LetterStats> letterStatsMap = new Dictionary<char, LetterStats>();
            stream.ResetPositionToStart();

            //Пара char для пары букв. Чтобы сравнивать.
            char c1, c2;

            try
            {
                c1 = Convert.ToChar(stream.ReadNextChar().ToString().ToUpper());
                while (!stream.IsEof)
                {
                    c2 = Convert.ToChar(stream.ReadNextChar().ToString().ToUpper());

                    if (c1 == c2)
                    {
                        if (letterStatsMap.ContainsKey(c1))
                        {
                            LetterStats tmp = letterStatsMap[c1];
                            IncStatistic(ref tmp);
                            letterStatsMap[c1] = tmp;
                        }
                        else if (Char.IsLetter(c1))
                        {
                            letterStatsMap.Add(c1, new LetterStats()
                            {
                                Count = 1,
                                Letter = c1.ToString() + c2.ToString()
                            });
                        }

                        c1 = Convert.ToChar(stream.ReadNextChar().ToString().ToUpper());
                    }
                    else
                    {
                        c1 = c2;
                    }
                }
            }
            catch (EndOfStreamException e)
            {
                Console.WriteLine(e);
            }

            return letterStatsMap.Select(item => item.Value).ToList();
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        public static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            if (letters == null) return;

            switch (charType)
            {
                case CharType.Consonants:
                    for (int i = 0; i < letters.Count; i++)
                    {
                        if (!IsVowel(letters[i].Letter[0]))
                        {
                            letters.RemoveAt(i);
                            i--;
                        }
                    }

                    break;
                case CharType.Vowel:
                    for (int i = 0; i < letters.Count; i++)
                    {
                        if (IsVowel(letters[i].Letter[0]))
                        {
                            letters.RemoveAt(i);
                            i--;
                        }
                    }

                    break;
                default:
                    Console.WriteLine("Invalid arg for remove type of letter");
                    break;
            }
        }

        private static bool IsVowel(char letter)
        {
            return "AaEeIiOoUuYy".Contains(letter);
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        public static void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            if (letters == null) return;

            List<LetterStats> letterStatsList = new List<LetterStats>(letters);
            letterStatsList.Sort(Comparison);

            Console.WriteLine();
            foreach (var letter in letterStatsList)
            {
                Console.WriteLine($"{letter.Letter} : {letter.Count.ToString()}");
            }
        }

        private static int Comparison(LetterStats x, LetterStats y)
        {
            return string.Compare(x.Letter, y.Letter, StringComparison.Ordinal);
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(ref LetterStats letterStats)
        {
            letterStats.Count++;
        }
    }
}