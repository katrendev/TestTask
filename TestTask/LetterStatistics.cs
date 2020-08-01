using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    /// <summary>
    /// Обертка для хранения списка ключей с частотой их вхождения
    /// удаление тут пока реализовывать не буду, потому что осталной код завязан на IList
    /// </summary>
    public class LetterStatistics
    {
        private Dictionary<string, int> statistics = new Dictionary<string, int>();

        public void IncreaseStatistic(string letter)
        {
            if (statistics.ContainsKey(letter))
            {
                statistics[letter]++;
            }
            else
            {
                statistics[letter] = 1;
            }
        }

        public IList<LetterStats> GetStatsList() =>
            statistics.Select(pair => new LetterStats {Letter = pair.Key, Count = pair.Value}).ToList();
    }
}