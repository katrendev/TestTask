using System;
using System.IO;
using System.Text;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private StreamReader  _streamReader;

        /// <summary>
        ///     Конструктор класса.
        /// </summary>
        public ReadOnlyStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof (stream));
            if (!stream.CanRead)
                throw new ArgumentException("Argument Stream Not Readable");
            Init(stream);
        }
        
        private void Init(Stream stream)
        {
            _streamReader = new StreamReader(stream);
            IsEof = false;
        }

        /// <summary>
        ///     Флаг окончания файла.
        /// </summary>
        public bool IsEof { get; private set; }

        /// <summary>
        ///     Ф-ция чтения следующего символа из потока.
        ///     Если произведена попытка прочитать символ после достижения конца файла, метод
        ///     должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            if (IsEof)
            {
                throw new EndOfStreamException();
            }
            
            var nextChar = (char) _streamReader.Read();

            if (_streamReader.EndOfStream)
            {
                IsEof = true;
            }

            return nextChar;
        }

        /// <summary>
        ///     Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_streamReader == null)
            {
                return;
            }
            
            _streamReader.DiscardBufferedData();
            _streamReader.BaseStream.Seek(0, SeekOrigin.Begin); 
            
            IsEof = false;
        }
        
        /// <summary>
        ///     Закрытие читающего потока
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            _streamReader?.Dispose();
        }
    }
}