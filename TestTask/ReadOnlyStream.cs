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
            IsEof = false;
            _localStream = new StreamReader(fileFullPath);


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
            char symb;
            if (IsEof == false)
            {
                if (_localStream.EndOfStream)
                {
                    IsEof = true;
                    symb = '~';
                    CloseFile();
                    return symb;
                }
                else
                {
                    return (char)_localStream.Read();
                }
            }
            else
            {
                throw new ArgumentException("попытка прочитать символ после достижения конца файла");
            }
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {

            _localStream.BaseStream.Position = 0;
            IsEof = false;

        }
        public void CloseFile()
        {
            _localStream.Dispose();
        }
    }
}
