using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestTask.Services.Interfaces;
using System.IO;

namespace TestTask.Services.Tests
{
    [TestClass()]
    public class ReadOnlyStreamReaderTests
    {
        [TestMethod()]
        public void ReadOnlyStreamReaderTest_UncorrectPath()
        {
            string path = @"D:\abcd\cde\1.txt";
            Assert.ThrowsException<DirectoryNotFoundException>(()=>new ReadOnlyStreamReader(path));
        }

        [TestMethod()]
        public void IsEofTest_EmptyFile_EofIsTrue()
        {
            string path = @".\TestFiles\Empty.txt";
            using (IReadOnlyStream stream = new ReadOnlyStreamReader(path))
                Assert.IsTrue(stream.IsEof);
        }

        [TestMethod()]
        public void ReadNextCharTest_EmptyFile_ThrowException()
        {
            string path = @".\TestFiles\Empty.txt";
            using (IReadOnlyStream stream = new ReadOnlyStreamReader(path))
                Assert.ThrowsException<EndOfStreamException>(() => stream.ReadNextChar());
        }

        [TestMethod()]
        public void ReadNextCharTest_OneSymbolFile_A()
        {
            string path = @".\TestFiles\OneSymbol.txt";
            using (IReadOnlyStream stream = new ReadOnlyStreamReader(path))
            {
                char expected = 'A';
                char actual = stream.ReadNextChar();
                Assert.AreEqual(expected,actual);
            }
        }

        [TestMethod()]
        public void ResetPositionToStartTest_TestTaskKatren_T()
        {
            string path = @".\TestFiles\TestTask Katren.txt";
            using (IReadOnlyStream stream = new ReadOnlyStreamReader(path))
            {
                char expected = 'T';
                stream.ResetPositionToStart();
                while (!stream.IsEof)
                    stream.ReadNextChar();

                stream.ResetPositionToStart();
                char actual = stream.ReadNextChar();
                Assert.AreEqual(expected, actual);
            }
        }
        [TestMethod()]
        public void DisposeTest_ObjectDisposedException()
        {
            string path = @".\TestFiles\TestTask Katren.txt";
            IReadOnlyStream stream = new ReadOnlyStreamReader(path);
            stream.Dispose();
            Assert.ThrowsException<ObjectDisposedException>(() => stream.ReadNextChar());
            Assert.ThrowsException<ObjectDisposedException>(() => stream.IsEof);
            Assert.ThrowsException<ObjectDisposedException>(() => stream.ResetPositionToStart());
        }
    }
}