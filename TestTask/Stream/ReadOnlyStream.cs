using System;
using System.IO;

namespace TestTask
{
    /// <summary>
    /// Представляет чтение файла в виде потока с возможностью только чтения.
    /// </summary>
    public class ReadOnlyStream : IReadOnlyStream
    {
        private readonly Stream _localStream;
        private readonly StreamReader _reader;

        /// <summary>
        /// Конструктор класса <see cref="ReadOnlyStream"/>.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения.</param>
        public ReadOnlyStream(string fileFullPath)
        {
            _localStream = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read);
            _reader = new StreamReader(_localStream);
        }

        /// <summary>
        /// Флаг, указывающий на окончание файла.
        /// </summary>
        public bool IsEof => _reader.EndOfStream;

        /// <summary>
        /// Читает следующий символ из потока.
        /// </summary>
        /// <returns>Считанный символ.</returns>
        /// <exception cref="EndOfStreamException">Бросается при достижении конца потока.</exception>
        public char ReadNextChar()
        {
            if (IsEof)
            {
                throw new EndOfStreamException("Достигнут конец файла.");
            }

            int nextChar = _reader.Read();
            return Convert.ToChar(nextChar);
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart() => _localStream.Seek(0, SeekOrigin.Begin);

        /// <summary>
        /// Освобождает ресурсы, связанные с чтением файла.
        /// </summary>
        public void Dispose()
        {
            _reader.Dispose();
            _localStream.Dispose();
        }
    }
}
