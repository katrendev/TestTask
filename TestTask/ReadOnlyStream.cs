using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private FileStream _localStream;
        private StreamReader reader;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            _localStream = File.OpenRead(fileFullPath);
            reader = new StreamReader(_localStream);
            IsEof = reader.EndOfStream;
        }
                
        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get;
            private set;
        }

        public void Close()
        {
            reader.Close();
            reader.Dispose();
            reader = null;
            _localStream.Close();
            _localStream.Dispose();
            _localStream = null;
            IsEof = true;
        }

        public void Dispose()
        {
            _localStream.Dispose();
            reader.Dispose();
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
                throw new Exception("Stream is over");
            }
            var ch = (char)reader.Read();
            IsEof = reader.Peek() == -1;
            return ch;
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
            reader.DiscardBufferedData();
            IsEof = false;
        }
    }
}
