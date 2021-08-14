using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
     
        private bool _disposed;
        private readonly StreamReader _localStream;

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
        /// Флаг окончания файла
        /// </summary>
        public bool IsEof 
        {
            get;
            private set;
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ</returns>
        public char ReadNextChar()
        {
            if (IsEof)
            {
                throw new IOException("Файл был прочитан!!!");
            }

            char ch = Convert.ToChar(_localStream.Read());
            IsEof = _localStream.EndOfStream;

            return ch;
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_localStream == null)
            {
                IsEof = true;
                return;
            }

            _localStream.DiscardBufferedData();
            _localStream.BaseStream.Seek(0, SeekOrigin.Begin);

            IsEof = false;
        }

        /// <summary>
        /// Очистка ресурсов
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _localStream.Dispose();
                _disposed = true;
            }
        }
    }
}
