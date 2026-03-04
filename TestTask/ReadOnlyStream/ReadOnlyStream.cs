using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private Stream _localStream;
        private StreamReader _localReader;

        private bool _isEof = false;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(Stream localStream, StreamReader localReader)
        {
            IsEof = false;

            _localStream = localStream;
            _localReader = localReader;            
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
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            if (IsEof)
            {
                throw new EndOfStreamException("Попытка чтения файла после достижения границы");
            }

            int symbol = _localReader.Read();
            if (symbol == -1)
            {
                IsEof = true;
            }

            return (char)symbol;
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

            _localStream.Position = 0;
            _localReader.DiscardBufferedData();
            IsEof = false;
        }

        /// <summary>
        /// Закрывает потоки для работы с файлом
        /// </summary>
        public void Dispose()
        {
            if (_localReader != null)
            {
                _localReader.Close();
            }
            if (_localStream != null)
            {
                _localStream.Close();
            }
        }
    }
}
