using System.Collections.Generic;

namespace TestTask
{
    internal class LetterAnalyzer
    {
        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        public IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            stream.ResetPositionToStart();

            Dictionary<char, LetterStats> letterStats = new Dictionary<char, LetterStats>();

            char c = stream.ReadNextChar();

            while (!stream.IsEof)
            {
                if (!letterStats.ContainsKey(c))
                {
                    letterStats[c] = new LetterStats(c.ToString(), 1);
                }
                else
                {
                    LetterStats stats = letterStats[c];
                    letterStats[c] = new LetterStats(stats.Letter, stats.Count + 1);
                }

                c = stream.ReadNextChar();
            }

            return new List<LetterStats>(letterStats.Values);
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        public IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            stream.ResetPositionToStart();

            Dictionary<string, LetterStats> letterStats = new Dictionary<string, LetterStats>();

            char buffer = stream.ReadNextChar();

            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();

                if (stream.IsEof)
                {
                    break;
                }

                string s1 = c.ToString().ToUpper();
                string s2 = buffer.ToString().ToUpper();

                if (s1 == s2)
                {
                    string pair = s1 + s2;
                    if (!letterStats.ContainsKey(pair))
                    {
                        letterStats[pair] = new LetterStats(pair, 1);
                    }
                    else
                    {
                        LetterStats stats = letterStats[pair];
                        letterStats[pair] = new LetterStats(stats.Letter, stats.Count + 1);
                    }

                    buffer = stream.ReadNextChar();
                }
                else
                {
                    buffer = c;
                }
            }

            return new List<LetterStats>(letterStats.Values);
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private void IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
        }

    }
}