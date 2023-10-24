using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private const string EndFileMessage = "The file has reched the end it's impossible to read char";

        private StreamReader _localStream;

        public ReadOnlyStream(string fileFullPath) => _localStream = new StreamReader(fileFullPath);

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEndOfStream => _localStream.EndOfStream;

        /// <summary>
        /// Считать следующий символ
        /// </summary>
        /// <returns>Считанный символ.</returns>
        /// <exception cref="EndOfStreamException">Выдаст исключение если попытаться считать символ после завершения</exception>
        public char ReadNextChar()
        {
            if (IsEndOfStream)
            {
                throw new EndOfStreamException(EndFileMessage);
            }

            return (char)_localStream.Read();
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_localStream != null)
            {
                _localStream.BaseStream.Position = 0;
                _localStream.DiscardBufferedData();
            }
        }

        public void Dispose() => _localStream.Dispose();
    }
}
