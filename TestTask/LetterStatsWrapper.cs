using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    public static class LetterStatsWrapper
    {
        /// <summary>
        ///     Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        ///     Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        public static IEnumerable<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            var letterStatses = new Dictionary<char, LetterStats>();
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                var ch = stream.ReadNextChar();

                if (!char.IsLetter(ch))
                {
                    continue;
                }

                if (!letterStatses.ContainsKey(ch))
                {
                    letterStatses[ch] = new LetterStats(ch);
                }

                var letterStats = letterStatses[ch];
                letterStats.IncCount();
            }

            return letterStatses.Values;
        }

        /// <summary>
        ///     Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        ///     В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        ///     Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        public static IEnumerable<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            var pairStatses = new Dictionary<char, LetterStats>();
            stream.ResetPositionToStart();

            var prevChar = '\0';

            while (!stream.IsEof)
            {
                var currChar = stream.ReadNextChar();

                if (!char.IsLetter(currChar))
                {
                    prevChar = '\0';
                    continue;
                }

                currChar = char.ToLower(currChar);

                if (prevChar != currChar)
                {
                    prevChar = currChar;
                    continue;
                }

                if (!pairStatses.ContainsKey(currChar))
                {
                    pairStatses[currChar] = new LetterStats(currChar);
                }

                var letterStats = pairStatses[currChar];
                letterStats.IncCount();
            }

            return pairStatses.Values;
        }

        /// <summary>
        ///     Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        ///     (Тип букв для перебора определяется параметром charType)
        ///     Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="letterType">Тип букв для анализа</param>
        public static void RemoveCharStatsByType(List<LetterStats> letters, LetterType letterType)
        {
            var alphabetProvider = new MultilingualAlphabetProvider();
            letters.RemoveAll(stats => stats.Letter.GetLetterType(alphabetProvider) == letterType);
        }

    }
}