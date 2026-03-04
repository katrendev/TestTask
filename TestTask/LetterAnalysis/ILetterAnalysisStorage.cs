using System.Collections.Generic;

namespace TestTask
{
    /// <summary>
    /// Интерфейс контейнера для подсчёта количества вхождений текста
    /// </summary>
    internal interface ILetterAnalysisStorage
    {
        void Add(string text);

        IList<LetterStats> GetStatistics();
    }
}
