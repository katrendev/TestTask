using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private Stream _localStream;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            if (!File.Exists(fileFullPath))
                throw new FileNotFoundException();
            _localStream = new FileStream(fileFullPath, FileMode.Open);
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof=>_localStream.Position == _localStream.Length;

        public void Close()
        {
            Dispose();
        }

        public static IReadOnlyStream GetInputStream(string fileFullPath)
        {
            return new ReadOnlyStream(fileFullPath);
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
                throw new EndOfStreamException();
            }

            return (char) _localStream.ReadByte();
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

        public void Dispose()
        {
            _localStream?.Dispose();
        }
    }
}