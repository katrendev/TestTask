using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private Stream _localStream;
        private StreamReader _streamReader;
        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            // TODO : Заменить на создание реального стрима для чтения файла!
            _localStream = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read);
            _streamReader = new StreamReader(_localStream);
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            // TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
            get {
                if (_localStream != null)
                    return _streamReader.EndOfStream;
                else
                    return true;
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
            // TODO : Необходимо считать очередной символ из _localStream           

            if (IsEof)
            {
                throw new EndOfStreamException();
            }

            var nextChar = (char)_streamReader.Read();

            return nextChar;
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
            _localStream.Dispose();
            _streamReader.Dispose();
        }
    }
}
