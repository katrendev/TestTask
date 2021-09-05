using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private readonly FileStream _localStream;
        private readonly BinaryReader _reader;
        private bool _isEof;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            _localStream = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read);
            _reader = new BinaryReader(_localStream);
            _isEof = _localStream.Position >= _localStream.Length;
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get => _isEof;
            private set => _isEof = value;
        }

        /// <summary>
        /// Освободить ресурсы занятые потоком
        /// </summary>
        public void Dispose()
        {
            _reader.Dispose();
            _localStream.Dispose();
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            if (_localStream.Position >= _localStream.Length)
            {
                throw new IOException("Нет данных для чтения из потока");
            }

            var nextChar = _reader.ReadChar();

            if (_localStream.Position == _localStream.Length)
            {
                IsEof = true;
            }

            return nextChar;
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_localStream != null)
            {
                _localStream.Position = 0;
                IsEof = _localStream.Position >= _localStream.Length;
            }
        }
    }
}