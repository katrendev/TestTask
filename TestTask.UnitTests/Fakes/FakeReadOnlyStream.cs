using System.IO;
using System.Text;

namespace TestTask.UnitTests.Fakes
{
    public class FakeReadOnlyStream : ReadOnlyStream
    {
        public FakeReadOnlyStream(string inputData, Encoding encoding = default) : base(inputData, encoding)
        {
        }

        public override Stream CreateStream(string param)
        {
            try
            {
                byte[] bytes = Encoding.GetBytes(param);
                return new MemoryStream(bytes);
            }
            catch
            {
                return Stream.Null;
            }
        }
    }
}
