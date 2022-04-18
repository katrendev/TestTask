using System.Collections.Generic;
using System.Linq;
using TestTask.Util;

namespace TestTask.Stats
{
    /// <summary>
    /// Класс сборщика статистики вхождения букв в поток (регистрозависимо)
    /// </summary>
    public class PairLetterStatsCollector : LetterStatsCollector
    {
        public static readonly PairLetterStatsCollector Instance = new PairLetterStatsCollector();

        private PairLetterStatsCollector()
        {
        }

        private static readonly ISet<char> AllowedLetters = CharUtil.CyrillicLettersWithAnalogs
            .Concat(CharUtil.LatinLettersWithAnalogs)
            .ToHashSet();

        protected override bool IsCharAllowed(char ch) => AllowedLetters.Contains(ch);

        protected override char NormalizeChar(char ch) => char.ToUpperInvariant(ch.ToLatinAnalog());
    }
}