using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace TestTask
{
    public interface IReadLetterFromStream
    {
        IList<LetterStats> ReadFromStream(IReadOnlyStream stream);
    }
}