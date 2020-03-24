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
            IsEof = false;

            // TODO : Заменить на создание реального стрима для чтения файла!
            _localStream = File.OpenRead(fileFullPath);
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        private bool isEof;
        public bool IsEof
        {
            get // TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
            {
                if (_localStream.Position == _localStream.Length)
                {
                    isEof = true;
                    Dispose();
                }
                else isEof = false;
                return isEof;
            }
            private set
            {
                isEof = value;
            }
        }

        public void Dispose()
        {
            _localStream.Close();
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
            // TODO : Необходимо считать очередной символ из _localStream
            if (_localStream.CanRead)
            {
                int result = _localStream.ReadByte();
                if (result != -1)
                    return (char)result;
                else
                {
                    IsEof = true;
                    throw new EndOfStreamException("Достигнут конец файла");
                }
            }
            else throw new Exception("Поток не доступен для чтения!");
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
