using System;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Text;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private StreamReader _localStream;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// 
        public ReadOnlyStream(string fileFullPath): this (fileFullPath, Encoding.UTF8) { }
        public ReadOnlyStream(string fileFullPath, Encoding enc)
        {
            if (string.IsNullOrWhiteSpace(fileFullPath))
            {
                throw new ArgumentNullException(nameof(fileFullPath),"Ошибка. Пустой путь до файла");
            }
            if(!File.Exists(fileFullPath))
            {
                throw new FileNotFoundException("Ошибка. Файл не найден",fileFullPath);
            }

           _localStream = new StreamReader(fileFullPath, enc);
           IsEof=_localStream.EndOfStream;
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
            _localStream?.Dispose();
            _localStream= null;
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            if (_localStream == null)
            {
                throw new ObjectDisposedException(nameof(ReadOnlyStream));
            }
            if (IsEof)
            {
                throw new EndOfStreamException("Коней файла");
            }

            int data = _localStream.Read();
            if (data == -1)
            {
                IsEof = true;
                throw new EndOfStreamException("Коней файла");
            }

            IsEof = _localStream.EndOfStream;
            return (char)data;
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            if(_localStream == null)
            {
                IsEof=true;
                return;
            }
            _localStream.DiscardBufferedData();
            _localStream.BaseStream.Seek(0,SeekOrigin.Begin);
            IsEof = _localStream.EndOfStream;
        }
    }
}
