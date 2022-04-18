using System.Collections.Generic;
using System.Linq;
using TestTask.Util;

namespace TestTask.Stats
{
    /// <summary>
    /// Класс для анализа вхождений символа в поток
    /// </summary>
    public class LetterStatsCollectorPairs : LetterStatsCollector
    {
        public new static readonly LetterStatsCollectorPairs Instance = new LetterStatsCollectorPairs();

        private static readonly ISet<char> AllowedLetters = CharUtil.CyrillicLettersWithAnalogs
            .Concat(CharUtil.LatinLettersWithAnalogs)
            .ToHashSet();

        protected override bool IsCharAllowed(char ch) => AllowedLetters.Contains(ch);

        protected override char NormalizeChar(char ch) => char.ToUpperInvariant(ch.ToLatinAnalog());
    }
}