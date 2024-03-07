using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private StreamReader _localStream;
        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            IsEof = true;
            _localStream = new StreamReader(fileFullPath);
        }
        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get;
            private set;
        }
        /// <summary>
        /// Освобождение ресурсов 
        /// </summary>
        public void Dispose()
        {
            _localStream?.Dispose();
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
                throw new EndOfStreamException("Попытка прочитать символ после достижения конца файла.");
            }
            var c = (char)_localStream.Read();
            if (_localStream.EndOfStream)
            {
                IsEof = true;
            }
            return c;
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
            IsEof = _localStream.EndOfStream;
        }
    }
}
