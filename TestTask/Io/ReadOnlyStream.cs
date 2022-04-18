using System;
using System.IO;
using System.Text;

namespace TestTask.Io
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;

        private readonly Encoding _encoding;
        private readonly Stream _localStream;
        private StreamReader _streamReader;
        private bool _isDisposed = false;

        /// <summary>
        /// Базовый конструктор ReadOnlyStream из любого абстрактного потока
        /// </summary>
        /// <param name="stream">Входной поток</param>
        /// <param name="encoding">Кодировка потока</param>
        public ReadOnlyStream(Stream stream, Encoding encoding = null)
        {
            _localStream = stream;
            _encoding = encoding ?? DefaultEncoding;
            InitReader();
        }

        /// <summary>
        /// Конструктор ReadOnlyStream из любого файла
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <param name="encoding">Кодировка файла</param>
        public ReadOnlyStream(string fileFullPath, Encoding encoding = null)
            : this(new FileStream(fileFullPath, FileMode.Open), encoding)
        {
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof => _isDisposed || _streamReader.Peek() == -1;

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            CheckDisposed();
            var ch = _streamReader.Read();
            if (ch == -1) throw new EndOfStreamException();
            return (char) ch;
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// <exception cref="NotSupportedException">Если внутренний поток не поддерживает перемотку</exception>
        /// </summary>
        public void ResetPositionToStart()
        {
            CheckDisposed();
            if (_localStream.CanSeek)
                _localStream.Seek(0, SeekOrigin.Begin);
            else
                throw new NotSupportedException("Inner stream does not support seek");
            // У ридера нет возможности seek'ать, поэтому просто заменяем его на новый,
            // а старый ридер НЕ диспозим, т.к. он прикроет основной стрим, а также в нём нет неуправляемых ресурсов
            InitReader();
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;
            _isDisposed = true;
            _localStream?.Dispose();
        }

        private void InitReader()
        {
            _streamReader = new StreamReader(_localStream, _encoding);
        }

        private void CheckDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Stream reader was disposed");
        }
    }
}