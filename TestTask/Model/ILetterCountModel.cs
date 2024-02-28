using System.Collections.Generic;

namespace TestTask
{
    public interface ILetterCountModel
    {
        void ReadStream(string filePath);
        IList<LetterStats> GetLetterStats();
        void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType);
    }
}
