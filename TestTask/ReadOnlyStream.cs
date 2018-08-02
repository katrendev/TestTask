using System;
using System.IO;
using System.Text;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream, IDisposable
    {
        private StreamReader  _streamReader;

        public ReadOnlyStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof (stream));
            if (!stream.CanRead)
                throw new ArgumentException("Argument Stream Not Readable");
            Init(stream);
        }
        
        private void Init(Stream stream)
        {
            _streamReader = new StreamReader(stream);
            IsEof = false;
        }

        public bool IsEof { get; private set; }

        public char ReadNextChar()
        {
            if (IsEof)
            {
                throw new EndOfStreamException();
            }
            
            var nextChar = (char) _streamReader.Read();

            if (_streamReader.EndOfStream)
            {
                IsEof = true;
            }

            return nextChar;
        }

        public void ResetPositionToStart()
        {
            if (_streamReader == null)
            {
                return;
            }
            
            _streamReader.DiscardBufferedData();
            _streamReader.BaseStream.Seek(0, SeekOrigin.Begin); 
            
            IsEof = false;
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            _streamReader?.Dispose();
        }
    }
}