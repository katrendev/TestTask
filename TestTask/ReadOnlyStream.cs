using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private StreamReader _localStream;
        private bool _disposed;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            fileFullPath = fileFullPath ?? throw new ArgumentNullException(nameof(fileFullPath));

            // TODO : Заменить на создание реального стрима для чтения файла!
            _localStream = new StreamReader(fileFullPath);
            IsEof = _localStream.EndOfStream;
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get; // TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
            private set;
        } = true;

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            // TODO : Необходимо считать очередной символ из _localStream
            if (IsEof)
            {
                throw new EndOfStreamException();
            }

            char readChar = (char)_localStream.Read();

            if (_localStream.EndOfStream)
            {
                IsEof = true;
            }

            return readChar;
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_localStream == null)
            {
                IsEof = true;
                return;
            }

            _localStream.BaseStream.Position = 0;
            _localStream.DiscardBufferedData();

            IsEof = _localStream.EndOfStream;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _localStream?.Dispose();
            }
            _disposed = true;
        }

        ~ReadOnlyStream()
        {
            Dispose(false);
        }
    }
}
