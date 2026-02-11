using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream, IDisposable
    {
        private bool _disposed;
        private TextReader _reader;
        private readonly Func<TextReader> _makeReader;

        /// <summary>
        /// Создает стрим для чтения из файла
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public static IReadOnlyStream ForFile(string fileFullPath)
        {
            return new ReadOnlyStream(() => new StreamReader(fileFullPath));
        }

        /// <summary>
        /// Создает стрим для чтения из строки (используется в тестах)
        /// </summary>
        public static IReadOnlyStream ForString(string content)
        {
            return new ReadOnlyStream(() => new StringReader(content));
        }

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="makeReader">Функия-фабрика ридеров</param>
        private ReadOnlyStream(Func<TextReader> makeReader)
        {
            _makeReader = makeReader;
            _reader = _makeReader();
        }

        public void Dispose()
        {
            if (_reader != null)
            {
                _reader.Dispose();
                _reader = null;
            }
            _disposed = true;
        }

        ~ReadOnlyStream()
        { 
            Dispose();
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get
            {
                ThrowIfDisposed();
                return _reader.Peek() == -1;
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
            ThrowIfDisposed();

            var charCode = _reader.Read();
            if (charCode < 0)
            {
                throw new EndOfStreamException();
            }

            return (char)charCode;
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            ThrowIfDisposed();

            // NOTE: Если бы использовать StreamReader напрямую, то можно было бы просто
            // _reader.BaseStream.Position = 0
            // _reader.DiscardBufferedData()
            // Но мы используем TextReader, чтобы проще писать тесты
            // поэтому проще всего пересоздать его
            _reader.Dispose();
            _reader = _makeReader();
        }
    }
}
