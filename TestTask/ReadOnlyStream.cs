using System.IO;
using System.Text;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private readonly StreamReader readerStream;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            IsEof = false;

            var _localStream = File.OpenRead(fileFullPath);

            readerStream = new StreamReader(_localStream, Encoding.UTF8);
        }
                
        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get;
            private set;
        }

        public void Dispose()
        {
            IsEof = true;
            readerStream.Close();
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            if (!IsEof)
            {
                var symbol = (char)readerStream.Read();

                if (readerStream.Peek() != -1)
                    readerStream.BaseStream.Position++;
                else
                    IsEof = true;

                return symbol;
            }

            throw new EndOfStreamException();
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            if (readerStream == null)
            {
                IsEof = true;
                return;
            }

            readerStream.BaseStream.Position = 0;
            IsEof = false;
        }
    }
}