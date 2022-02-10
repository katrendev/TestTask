using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private StreamReader _streamReader;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {            
            _streamReader = new StreamReader(fileFullPath);
        }

        public bool IsEof { get; private set; }

        public char ReadNextChar()
        {
            if (_streamReader == null || _streamReader.EndOfStream)
            {
                throw new EndOfStreamException("Error while trying to read a stream: the stream is null or stream has ended");
            }

            var nextChar = (char)_streamReader.Read();           

            if (_streamReader.EndOfStream)
            {
                IsEof = true;
            }

            return nextChar;
        }

        public void ResetPositionToStart()
        {
            if (_streamReader == null)
            {
                IsEof = true;
                return;
            }
            _streamReader.BaseStream.Position = 0;
            IsEof = false;
        }

        public void Dispose()
        {
            if (_streamReader != null)
            {
                _streamReader.Dispose();
            }         
        }
    }
}
