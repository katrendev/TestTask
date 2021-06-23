using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private readonly StreamReader _localStream;
        private bool _disposed;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            _localStream = new StreamReader(fileFullPath);
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof => _localStream.EndOfStream;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
                var c = (char)_localStream.Read();
                return c;
            }
            else
            {
                throw new InvalidOperationException("Попытка чтения после достижения конца файла");
            }
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            _localStream.BaseStream.Position = 0;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    try
                    {
                        _localStream.Close();//вызовет StreamReader.Dispose(true)
                    }
                    catch(ObjectDisposedException)
                    {
                        //Если закрыли через Close()
                    }
                }
                _disposed = true;
            }
        }

        public void Close()
        {
            _localStream.Close();
        }

        ~ReadOnlyStream()
        {
            Dispose(false);
        }
    }
}
