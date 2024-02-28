using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTask.View
{
    public class SingleConterView : ILetterConterView
    {
        public void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            var letterStatsEnumerable = letters as LetterStats[] ?? letters.ToArray();
            var orderedLetters = letterStatsEnumerable.OrderBy(x => x.Letter);

            Console.WriteLine("Статистика вхождения каждой буквы:");

            foreach (var letterStats in orderedLetters)
            {
                Console.WriteLine("{" + letterStats.Letter + "} " + ":" + " {" + letterStats.Count + "}");
            }
        }
    }
}
