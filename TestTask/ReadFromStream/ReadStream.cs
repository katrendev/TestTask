using System.Collections.Generic;

namespace TestTask
{
    public class ReadStream
    {
        private IReadLetterFromStream _stream;
        public ReadStream(IReadLetterFromStream stream)
        {
            _stream = stream;
        }

        public IList<LetterStats> Read(IReadOnlyStream stream)
        {
            return _stream.ReadFromStream(stream);
        }
    }
}