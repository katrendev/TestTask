using System;
using System.IO;
using System.Text;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private Stream _localStream;
        private bool isEof;
        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            IsEof = true;
            // TODO : Заменить на создание реального стрима для чтения файла!
            _localStream = new FileStream(fileFullPath, FileMode.Open);
        }

        public void CloseInputStream()
        {
            _localStream.Close();
        }


        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get
            {
                return isEof;
            }
            private set
            {
                isEof = value;
            }
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            if (_localStream.Position == _localStream.Length - 1)
            {
                IsEof = true;
            }
            return (char)_localStream.ReadByte();
        }

        public void ResetPosBack()
        {
            if (_localStream.Position != 0)
            {
                _localStream.Position--;
            }
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
