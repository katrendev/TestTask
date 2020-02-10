using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask
{
    public abstract class Statistics
    {
        protected List<LetterStats> statistics { get; set; } = new List<LetterStats>();
        protected IReadOnlyStream stream;

        protected Statistics(string fileFullPath)
        {
            stream = new ReadOnlyStream(fileFullPath);
            stream.ResetPositionToStart();
            FillStatistics();
        }
        protected abstract void FillStatistics();
        public void RemoveCharStatsByType(CharType charType)
        {
           statistics.RemoveAll(x=>x.CharType==charType);

        }
        public virtual void PrintStatistic()
        {
            if (statistics.Count==0)
                return;

            foreach (var letterstats in statistics.OrderBy(x=>x.Letter))
            {
                Console.WriteLine($"{letterstats.Letter} - {letterstats.Count}");
            }
        }
        

    }
}
