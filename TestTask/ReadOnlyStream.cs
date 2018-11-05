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
            StreamReader sr = new StreamReader(fileFullPath);
            _localStream = sr;
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
            if (_localStream == null || _localStream.EndOfStream)
            {
                throw new Exception("Error while trying to read a stream: the stream is null or stream has ended");
            }

            char nextChar = (char)_localStream.Read();           

            if (_localStream.EndOfStream)
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
            if (_localStream == null)
            {
                IsEof = true;
                return;
            }
            _localStream.BaseStream.Position = 0;
            IsEof = false;
        }

        /// <summary>
        /// Закрывает поток и высвобождает ресурсы.
        /// </summary>
        public void Dispose()
        {
            if (_localStream != null)
            {
                _localStream.Dispose();
            }         
        }
    }
}
