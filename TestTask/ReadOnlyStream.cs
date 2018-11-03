using System;
using System.IO;
using System.Text;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private readonly StreamReader _localStream;

        private bool _isDisposed;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            _localStream = new StreamReader(fileFullPath, Encoding.Default);
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get
            {
                CheckForDisposed();
                return _localStream.EndOfStream;
            }
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            CheckForDisposed();
            if (IsEof)
            {
                throw new EndOfStreamException("The end of file is reached");
            }

            return (char) _localStream.Read();
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            CheckForDisposed();
            _localStream.BaseStream.Position = 0;
        }

        public void Dispose()
        {
            _localStream.Dispose();
            _isDisposed = true;
        }

        private void CheckForDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("The stream is already disposed");
            }
        }
    }
}
