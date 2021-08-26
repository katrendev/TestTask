using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private readonly Stream _localStream;
        private readonly StreamReader _streamReader;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            try
            {
                _localStream = new FileStream(fileFullPath, FileMode.Open);
                _streamReader = new StreamReader(_localStream);
                IsEof = false;
            }
            catch (Exception)
            {
                Console.WriteLine($"{fileFullPath} not open");
                _localStream = Stream.Null;
                _streamReader = StreamReader.Null;
                IsEof = true;
            }
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof { get; private set; }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            char nextChar = (char) 0;

            if (!_streamReader.EndOfStream)
            {
                nextChar = (char) _streamReader.Read();
            }
            else
            {
                IsEof = true;
                throw new EndOfStreamException();
            }

            return nextChar;
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

        public void CloseStream()
        {
            _streamReader.Close();
            _localStream.Close();
        }
    }
}