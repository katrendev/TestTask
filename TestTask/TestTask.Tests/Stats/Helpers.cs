using System.IO;
using System.Text;
using TestTask.Io;

namespace TestTask.Tests.Stats
{
    public static class Helpers
    {
        public static IReadOnlyStream StreamFrom(string value, Encoding encoding = null)
        {
            var enc = encoding ?? Encoding.UTF8;
            return new ReadOnlyStream(new MemoryStream(enc.GetBytes(value)));
        }
    }
}