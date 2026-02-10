using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream, IDisposable
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
            IsEof = true;
            // TODO : Заменить на создание реального стрима для чтения файла!
            _localStream = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read, FileShare.Read);

        }
        ~ReadOnlyStream() 
        {
            Close();
        }

        private bool isEof
        {
            get; set;
        }
                
        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get 
            {
                return _localStream.Length <= _localStream.Position;
            } // TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
            private set
            {
                isEof = value;
            }
        }

        public void Close()
        {
            _localStream.Close();
        }

        public void Dispose()
        {
            _localStream?.Dispose();
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
            var readByte = _localStream.ReadByte();
            if(readByte == -1)
            {
                throw new EndOfStreamException();
            }
            return (char)readByte;
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
            
            byte[] bom = new byte[3];
            int n = _localStream.Read(bom, 0, bom.Length);

            //Handling BOM
            if (n == 3 && (bom[0] & 0xFF) == 0xEF && (bom[1] & 0xFF) == 0xBB && (bom[2] & 0xFF) == 0xBF)
            {
                _localStream.Position = bom.Length;
            }
            else
            {
                _localStream.Position = 0;
            }
            
            IsEof = false;
        }
    }
}
