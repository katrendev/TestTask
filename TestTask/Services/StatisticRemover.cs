using TestTask.Models;
using TestTask.Services.LetterStrategy;

namespace TestTask.Services;

public static class LetterStatsExtension
{
    /// <summary>
    /// Удалить записи по типу стратегии.
    /// </summary>
    /// <param name="letterStats">Список статистики.</param>
    /// <param name="strategy"> Тип стратегии.</param>
    public static void RemoveElementsByStrategy(this IList<LetterStats> letterStats, ICharFilterStrategy strategy)
    {
        for(int i = 0; i < letterStats.Count; i++)
        {
            if(!strategy.ShouldRemove(letterStats[i].Letter))
            {
                continue;
            }
            letterStats.RemoveAt(i);
            i--;
        }
    }
}