using System;
using System.IO;
using System.Text;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private Stream _localStream;
        private Encoding _encoding;


        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            IsEof = true;
            _encoding = Encoding.GetEncoding(1251);
            _localStream = new StreamReader(fileFullPath).BaseStream;
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof { get; private set; }

        public void Dispose()
        {
            _localStream.Dispose();
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            int b = _localStream.ReadByte();
            if (b == -1)
            {
                IsEof = true;
                return (char)0;
            }


            char c = Encoding.GetEncoding(1251).GetString(new byte[] { (byte)b })[0];
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
