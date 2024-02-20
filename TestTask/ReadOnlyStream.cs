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
            if (fileFullPath == null)
                throw new ArgumentNullException(nameof(fileFullPath), "File path cannot be null.");

            _streamReader = new StreamReader(fileFullPath);
        }

        /// <summary>
        /// Флаг, указывающий на окончание файла.
        /// </summary>
        public bool IsEof => _streamReader.EndOfStream;

        /// <summary>
        /// Читает следующий символ из потока.
        /// </summary>
        /// <returns>Считанный символ.</returns>
        /// <exception cref="EndOfStreamException">Бросается при достижении конца потока.</exception>
        public char ReadNextChar()
        {
            if (_streamReader == null || _streamReader.EndOfStream) throw new EndOfStreamException("End of stream reached.");

            using (var reader = _streamReader)
            {
                var nextChar = reader.Read();
                if (nextChar == -1) throw new EndOfStreamException("End of stream reached.");

                return (char)nextChar;
            }
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            _streamReader?.BaseStream.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// Освобождает ресурсы, связанные с чтением файла.
        /// </summary>
        public void Dispose()
        {
            if (_streamReader == null) return;

            _streamReader.Dispose();
            _streamReader = null;
        }
    }
}
