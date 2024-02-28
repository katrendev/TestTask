using System.Collections.Generic;

namespace TestTask.View
{
    public interface ILetterCountView
    {
        void PrintStatistic(IEnumerable<LetterStats> letters);
    }
}
