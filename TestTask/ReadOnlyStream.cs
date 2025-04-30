using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream, IDisposable
    {
        private Stream _localStream;
        private StreamReader _reader;

        public ReadOnlyStream(string fileFullPath)
        {
            try
            {
                _localStream = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read);
                _reader = new StreamReader(_localStream);
                IsEof = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при открытии файла: " + ex.Message);
                IsEof = true;
            }
        }

        public bool IsEof { get; private set; }

        public char ReadNextChar()
        {
            if (_reader == null)
            {
                throw new InvalidOperationException("Поток не инициализирован");
            }

            if (_reader.EndOfStream)
            {
                IsEof = true;
                throw new EndOfStreamException("Достигнут конец файла");
            }

            int value = _reader.Read();
            if (value == -1)
            {
                IsEof = true;
                throw new EndOfStreamException("Не удалось прочитать символ");
            }

            if (_reader.EndOfStream)
            {
                IsEof = true;
            }

            return (char)value;
        }

        public void ResetPositionToStart()
        {
            if (_localStream != null && _localStream.CanSeek)
            {
                _localStream.Position = 0;
                _reader.DiscardBufferedData(); // чтобы сбросить буфер
                IsEof = false;
            }
            else
            {
                IsEof = true;
            }
        }

        public void Dispose()
        {
            if (_reader != null)
            {
                _reader.Dispose();
                _reader = null;
            }

            if (_localStream != null)
            {
                _localStream.Dispose();
                _localStream = null;
            }
        }
    }
}
