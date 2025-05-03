using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private FileStream _fileStream;
        private bool _disposed;
        private readonly StreamReader _reader;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            _fileStream = File.OpenRead(fileFullPath);
            _reader = new StreamReader(_fileStream);
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        /// 
        // TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
        public bool IsEof
        {
            get
            {
                ThrowIfDisposed();
                return _reader.EndOfStream;
            }
        }


        public void Dispose()
        {
            if (_disposed) return;
            _reader.Dispose();
            _fileStream.Dispose();
            _disposed = true;
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            // TODO : Необходимо считать очередной символ из _localStream
            ThrowIfDisposed();

            int value = _reader.Read();
            if (value == -1)
            {
                throw new EndOfStreamException();
            }
            return (char)value;
        }



        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            ThrowIfDisposed();

            _reader.DiscardBufferedData();
            _fileStream.Position = 0;
        }
        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ReadOnlyStream));
        }
    }
}
