using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private readonly FileStream _fileStream;
        private readonly StreamReader _reader;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            _fileStream = File.OpenRead(fileFullPath);
            _reader = new StreamReader(_fileStream);

            ResetPositionToStart();
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
                throw new EndOfStreamException();

            int value = _reader.Read();

            if (value == -1)
            {
                IsEof = true;
                throw new EndOfStreamException();
            }

            if (_reader.Peek() == -1)
                IsEof = true;

            return (char)value;
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            _fileStream.Position = 0;
            _reader.DiscardBufferedData();
            IsEof = _reader.Peek() == -1;
        }

        public void Dispose()
        {
            _reader?.Dispose();
            _fileStream?.Dispose();
        }
    }
}
