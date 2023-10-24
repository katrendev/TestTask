using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream, IDisposable
    {
        private const string EndFileMessage = "The file has reched the end it's impossible to read char";
        private StreamReader _localStream;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            // TODO : Заменить на создание реального стрима для чтения файла!
            _localStream = File.OpenText(fileFullPath);
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEndOfStream => _localStream.EndOfStream;

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            // TODO : Необходимо считать очередной символ из _localStream
            if (IsEndOfStream)
            {
                throw new EndOfStreamException(EndFileMessage);
            }

            return (char)_localStream.Read();
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_localStream != null)
            {
                _localStream.BaseStream.Position = 0;
                _localStream.DiscardBufferedData();
            }
        }

        public void Dispose() => _localStream.Dispose();
    }
}
