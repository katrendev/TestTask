using System;
using System.IO;
using System.Text;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private Stream _localStream;

        private bool IsDisposed
        {
            get;
            set;
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            _localStream.Dispose();
            _localStream = null;

            IsDisposed = true;
        }

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
            var charCode = _localStream.ReadByte();

            if (_localStream.ReadByte() == -1)
            {
                IsEof = true;
                Dispose();
            }
            else
            {
                _localStream.Position--;

                if (IsControlByte((byte)charCode))
                {
                    var utfCharcode = new byte[] { (byte)charCode, (byte)_localStream.ReadByte() };
                    charCode = Convert.ToChar(Encoding.UTF8.GetString(utfCharcode));
                }
            }

            return (char)charCode;
        }

        public bool IsControlByte(byte b)
        {
            var controlBytesStartPosition = 128;
            return controlBytesStartPosition < b;
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
