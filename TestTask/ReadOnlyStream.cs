using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        // использовали StreamReader потому, что оно корректно работает с `char` и кодировкой
        private StreamReader _localStream;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            // при создании открываем файл на чтение
            _localStream = new StreamReader(fileFullPath);
            IsEof = false;
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
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            if (_localStream == null)
            {
                throw new ObjectDisposedException(nameof(ReadOnlyStream));
            }

            int result = _localStream.Read();
            if (result == -1)
            {
                IsEof = true;
                throw new EndOfStreamException("Attempted to read past end of stream");
            }

            return (char)result;
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

            _localStream.BaseStream.Seek(0, SeekOrigin.Begin);
            _localStream.DiscardBufferedData();
            IsEof = false;
        }

        /// <summary>
        /// Закрывает внутренний поток.
        /// </summary>
        public void Dispose()
        {
            _localStream?.Dispose();
            _localStream = null;
            IsEof = true;
        }
    }
}
