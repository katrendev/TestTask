using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private readonly StreamReader _fileStream;

        /// <summary>
        /// Конструктор класса.
        /// Т.к. происходит прямая работа с файлом, необходимо
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            _fileStream = new StreamReader(fileFullPath);
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
        /// бросит соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            if (IsEof)
            {
                throw new InvalidOperationException("Stream is already at end of file");
            }

            var nextChar = (char)_fileStream.Read();

            if (_fileStream.EndOfStream)
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
            if (_fileStream == null)
            {
                throw new InvalidOperationException("Stream is not initialized.");
            }

            _fileStream.BaseStream.Position = 0;
            _fileStream.DiscardBufferedData();
            IsEof = false;
        }
    }
}
