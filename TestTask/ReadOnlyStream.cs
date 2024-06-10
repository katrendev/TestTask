using System;
using System.IO;
using System.Text;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private readonly Stream _localStream;
        private readonly byte[] buffer = new byte[1024];
        private readonly string textFromFile;
        private int index = 0;
        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            _localStream = File.OpenRead(fileFullPath);
            buffer = new byte[_localStream.Length];
            _localStream.Read(buffer, 0, buffer.Length);
            textFromFile = Encoding.UTF8.GetString(buffer);
        }
        public void Dispose()
        {
            _localStream.Dispose();
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
            if (IsEof)
                throw new EndOfStreamException("End of file");

            if (index < textFromFile.Length)
            {
                char ch = textFromFile[index];
                index++;
                return ch;
            }
            IsEof = true;
            
            return ' ';
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
            IsEof = false;
        }
    }
}
