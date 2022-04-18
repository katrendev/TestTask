using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using TestTask.Io;

namespace TestTask.Tests.Io

{
    [TestFixture]
    public class ReadOnlyStreamTest
    {
        [Test]
        public void TestStreamFullReadWithDefaultEncoding()
        {
            const string testData = "Some string";
            string result;
            using (var stream = new ReadOnlyStream(new MemoryStream(Encoding.UTF8.GetBytes(testData))))
            {
                result = ReadFully(stream);
            }

            Assert.AreEqual(testData, result);
        }

        [Test]
        public void TestStreamWithCustomEncoding()
        {
            const string testData = "Это тестовая строка";
            var cp1251 = Encoding.GetEncoding("windows-1251");
            string result;
            using (var stream = new ReadOnlyStream(new MemoryStream(cp1251.GetBytes(testData)), cp1251))
            {
                result = ReadFully(stream);
            }

            Assert.AreEqual(testData, result);
        }

        [Test]
        public void TestStreamRewind()
        {
            const string testData = "Some string";
            var chars = new List<char>();
            using (var stream = new ReadOnlyStream(new MemoryStream(Encoding.UTF8.GetBytes(testData))))
            {
                var count = 5;
                while (stream.IsEof)
                {
                    chars.Add(stream.ReadNextChar());
                    count -= 1;
                    if (count == 0)
                        stream.ResetPositionToStart();
                }
            }

            Assert.AreEqual("Some Some string", string.Join("", chars));
        }

        [Test]
        public void TestStreamRewindThrowsIfSeekIsNotSupported()
        {
            const string testData = "Some string";
            using (var stream = new ReadOnlyStream(new UnseekableMemoryStream(Encoding.UTF8.GetBytes(testData))))
            {
                Assert.Throws<NotSupportedException>(() => stream.ResetPositionToStart());
            }
        }

        [Test]
        public void TestStreamDisposesUnderlyingStream()
        {
            var innerStream = new MemoryStream(Array.Empty<byte>());
            new ReadOnlyStream(innerStream).Dispose();

            Assert.Throws<ObjectDisposedException>(() => { innerStream.Read(Array.Empty<byte>(), 0, 0); });
        }

        [Test]
        public void TestStreamBehaviourAfterDisposed()
        {
            var stream = new ReadOnlyStream(new MemoryStream(Array.Empty<byte>()));
            stream.Dispose();

            Assert.AreEqual(true, stream.IsEof);
            Assert.Throws<ObjectDisposedException>(() => stream.ResetPositionToStart());
            Assert.Throws<ObjectDisposedException>(() => stream.ReadNextChar());
            Assert.DoesNotThrow(() => stream.Dispose());
        }

        [Test]
        public void TestFileRead()
        {
            var file = Path.GetTempFileName();
            File.WriteAllText(file, "Test data"); // По дефолту пишется в UTF8

            string result;
            using (var stream = new ReadOnlyStream(file))
            {
                result = ReadFully(stream);
            }

            Assert.AreEqual("Test data", result);
        }

        private static string ReadFully(IReadOnlyStream stream)
        {
            var builder = new StringBuilder();
            while (stream.IsEof)
            {
                builder.Append(stream.ReadNextChar());
            }

            return builder.ToString();
        }

        private class UnseekableMemoryStream : MemoryStream
        {
            public UnseekableMemoryStream(byte[] buffer) : base(buffer)
            {
            }

            public override bool CanSeek => false;
        }
    }
}