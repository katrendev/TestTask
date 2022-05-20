using System;
using System.IO;
using System.Text;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private readonly Stream _localStream;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            // TODO: remove
            //IsEof = true;

            // TODO - Done : Заменить на создание реального стрима для чтения файла!
            // Done
            _localStream = new FileStream(fileFullPath, FileMode.Open);
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        // TODO - Done : Заполнять данный флаг при достижении конца файла/стрима при чтении
        public bool IsEof => _localStream.Position == _localStream.Length;
        // TODO: remove
        //public bool IsEof
        //{
        //    get; // TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
        //    private set;
        //}

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            // TODO - Done : Необходимо считать очередной символ из _localStream
            if (IsEof)
                throw new EndOfStreamException("File is completely read");

            return (char) _localStream.ReadByte();
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_localStream == null)
            {
                // TODO: remove
                //IsEof = true;
                return;
            }

            _localStream.Position = 0;
            // TODO: remove
            //IsEof = false;
        }

        public void Dispose()
        {
            _localStream?.Dispose();
        }
    }
}
