namespace TestTaskTests;

using TestTask;

[TestClass]
public class ReadOnlyStreamTests
{
    [TestMethod]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void ReadNextChar_AfterDispose_RaisesException()
    {
        IReadOnlyStream stream;
        using (stream = ReadOnlyStream.ForString("A"))
        {
            // pass
        }
        stream.ReadNextChar();
    }

    [TestMethod]
    public void ReadNextChar_WhenEmptyContent_DoesNothing()
    {
        string res = "";
        using (var stream = ReadOnlyStream.ForString(""))
        {
            while (!stream.IsEof)
            {
                res += stream.ReadNextChar();
            }
        }
        Assert.AreEqual("", res);
    }

    [TestMethod]
    public void ReadNextChar_WhenHasContent_ReadsAllChars()
    {
        string res = "";
        using (var stream = ReadOnlyStream.ForString("a1!"))
        {
            while (!stream.IsEof)
            {
                res += stream.ReadNextChar();
            }
        }
        Assert.AreEqual("a1!", res);
    }

    [TestMethod]
    public void ResetPositionToStart_AllowsForRereading()
    {
        string res = "";
        using (var stream = ReadOnlyStream.ForString("ab?"))
        {
            while (!stream.IsEof)
            {
                res += stream.ReadNextChar();
            }
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                res += stream.ReadNextChar();
            }
        }
        Assert.AreEqual("ab?ab?", res);
    }
}
