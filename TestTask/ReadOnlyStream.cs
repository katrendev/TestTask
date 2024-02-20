using System;
using System.IO;

namespace TestTask
{
    /// <summary>
    /// Представляет чтение файла в виде потока с возможностью только чтения.
    /// </summary>
    public class ReadOnlyStream : IReadOnlyStream
    {
        private StreamReader _streamReader;

        /// <summary>
        /// Конструктор класса <see cref="ReadOnlyStream"/>.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения.</param>
        public ReadOnlyStream(string fileFullPath)
        {
            try
            {
                _streamReader = new StreamReader(fileFullPath);
                IsEof = false;
            }
            catch (Exception)
            {
                IsEof = true;
                throw;
            }
        }

        /// <summary>
        /// Флаг, указывающий на окончание файла.
        /// </summary>
        public bool IsEof { get; private set; }

        /// <summary>
        /// Читает следующий символ из потока.
        /// </summary>
        /// <returns>Считанный символ.</returns>
        /// <exception cref="EndOfStreamException">Бросается при достижении конца потока.</exception>
        public char ReadNextChar()
        {
            if (IsEof)
                throw new EndOfStreamException("End of stream reached.");

            int nextChar = _streamReader.Read();
            if (nextChar == -1)
            {
                IsEof = true;
                throw new EndOfStreamException("End of stream reached.");
            }

            return (char)nextChar;
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_streamReader == null)
            {
                IsEof = true;
                return;
            }

            _streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            IsEof = false;
        }

        /// <summary>
        /// Освобождает ресурсы, связанные с чтением файла.
        /// </summary>
        public void Dispose()
        {
            _streamReader?.Dispose();
        }
    }
}
