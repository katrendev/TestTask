using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private StreamReader _localStream = null;
        private bool disposed = false;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            IsEof = true;            
            _localStream = null;

            // TODO : Заменить на создание реального стрима для чтения файла!
            if (!File.Exists(fileFullPath))
            {
                throw new FileNotFoundException($"Файл не существует {fileFullPath}");
            }

            // Пробуем открыть файл на чтение
            _localStream = new StreamReader(fileFullPath);
        }
            
        /// <summary>
        /// Закрытие потока по требованию
        /// </summary>
        public void Close()
        {
            IsEof = true;

            if (_localStream != null)
            {                
                _localStream.Dispose();
                _localStream = null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                Close();

                disposed = true;

                if (disposing)
                {                 
                    GC.SuppressFinalize(this);
                }
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                Dispose(true);
            }
        }

        ~ReadOnlyStream()
        {            
            Dispose(false);         
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            // TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
            get
            {
                if (_localStream != null)
                {
                    return _localStream.EndOfStream;
                } else
                {
                    return true;
                }
            }

            private set { _ = value; }
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока. Дефолтное значение \0
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            // TODO : Необходимо считать очередной символ из _localStream
            char r = char.MinValue;

            if (_localStream != null)
            {
                int k = _localStream.Read();
                if (k != - 1)
                {
                    r = (char)k;
                } 
                else
                {
                    throw new NotCharsInStream();
                }
            } 

            return r;            
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

            _localStream.BaseStream.Position = 0;
            _localStream.DiscardBufferedData();
            IsEof = false;
        }
    }

    /// <summary>
    /// Исключение - при чтении символов закончился поток
    /// </summary>
    public class NotCharsInStream: Exception
    {
        public NotCharsInStream()
        { }

        public NotCharsInStream(string message)
            : base(message)
        { }
    }

}
