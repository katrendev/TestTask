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
            //IsEof = true;

            // TODO : Заменить на создание реального стрима для чтения файла!
            if (!File.Exists(fileFullPath))
                throw new FileNotFoundException("File not found at path: " + fileFullPath);

            _localStream = File.OpenRead(fileFullPath);
            _streamReader = new StreamReader(_localStream, System.Text.Encoding.Default);
        }
                
        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get { return _streamReader.EndOfStream; } // TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
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
                throw new EndOfStreamException();

            // TODO : Необходимо считать очередной символ из _localStream
            return (char)_streamReader.Read();
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

        void IDisposable.Dispose()
        {
            _streamReader.Dispose();
        }
    }
}
