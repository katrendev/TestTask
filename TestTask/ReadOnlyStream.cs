using System;
using System.Text;
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
            IsEof = true;

            _localStream = File.OpenRead(fileFullPath);

            IsEof = _localStream == null;
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof { get; private set;  } = true;

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
            char c = '\0';
            if (!IsEof)
            {
                c = Encoding.GetEncoding(1251).GetChars(BitConverter.GetBytes(_localStream.ReadByte()))[0];
            }
            if (_localStream.Position >= _localStream.Length)
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

            _localStream.Position = 0;
            IsEof = false;
        }
    }
}
