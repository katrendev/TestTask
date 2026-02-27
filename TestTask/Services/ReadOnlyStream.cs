using System.Text;
using TestTask.Interfaces;

namespace TestTask.Services
{
    public sealed class ReadOnlyStream : IReadOnlyStream
    {
        private StreamReader _localStream;

        /// <summary>
        /// Конструктор класса. 
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            var fileStream = File.Open(fileFullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            _localStream = new StreamReader(fileStream, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof => _localStream == null || _localStream.EndOfStream;

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
             if(IsEof)
                 throw new EndOfStreamException();
             
             return (char)_localStream.Read();
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

            _localStream.DiscardBufferedData();
            _localStream.BaseStream.Position = 0;
        }

        public void Dispose()
        {
            _localStream?.Dispose();
            _localStream = null;
        }
    }
}
