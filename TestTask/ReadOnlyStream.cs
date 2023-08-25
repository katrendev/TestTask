using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private readonly char DefaultChar = '\0';
        private Stream _localStream;
        private bool _closedStream = false;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            _localStream = File.OpenRead(fileFullPath);
        }

        ~ReadOnlyStream()
        {
            if (_closedStream)
            {
                return;
            }
            CloseStream();
        }

        public void Dispose()
        {
            CloseStream();
        }

        public void CloseStream()
        {
            if (_closedStream)
            {
                return;
            }

            _localStream.Close();
            _closedStream = true;
        }
                
        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get => _localStream == null || _localStream.Position == _localStream.Length;
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            if (!TryGetNextChar(out char nextChar))
            {
                throw new EofException();
            }
            
            return nextChar;
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_localStream == null)
            {
                return;
            }

            _localStream.Position = 0;
        }

        private bool TryGetNextChar(out char nextChar)
        {
            nextChar = DefaultChar;
            if(_localStream == null)
            {
                return false;
            }
            var nextByte = _localStream.ReadByte();

            nextChar = (char)nextByte;
            return true;
        }
    }
}
