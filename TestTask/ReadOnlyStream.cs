using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private Stream _localStream;
        private StreamReader _localStreamReader;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            _localStream = File.Open(fileFullPath, FileMode.Open, FileAccess.Read);
            _localStreamReader = new StreamReader(_localStream);
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof() => _localStreamReader.EndOfStream;

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            if (_localStreamReader.EndOfStream)
            {
                throw new EndOfStreamException();
            }
            
            return (char)_localStreamReader.Read();
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            _localStream.Position = 0;
        }

        public void FileClose()
        {
            _localStream.Close();
            _localStreamReader.Close();
        }
    }
}
