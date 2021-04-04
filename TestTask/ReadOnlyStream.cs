using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private StreamReader _localStreamReader;
        private string _fileFullPath;
        private bool _disposed = false;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            IsEof = false;

            // TODO : Заменить на создание реального стрима для чтения файла!
            _fileFullPath = fileFullPath;
            _localStreamReader = new StreamReader(_fileFullPath);
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get; // TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
            private set;
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
            if (!IsEof)
            {
                char result = (char)_localStreamReader.Read();
                if (_localStreamReader.Peek() < 0)
                {
                    IsEof = true;
                }
                return result;
            }
            else
            {
                throw new InvalidOperationException("Достигнут конец файла");
            }
        }
        /// <summary>
        /// Ф-ция получения следующего символа из потока без перевода позиции. 
        /// Если следующий символ отсутствует - возвращает значение по умолчанию.
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char GetNextChar()
        {
            if (_localStreamReader.Peek() > 0)
            {
                return (char)_localStreamReader.Peek();
            }
            else
            {
                return new char();
            }
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            _localStreamReader = new StreamReader(_fileFullPath);

            IsEof = false;
        }

        /// <summary>
        /// Реализация интерфейса IDisposable.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // подавляем финализацию
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Освобождаем управляемые ресурсы
                    _localStreamReader?.Close();
                }
                // Освобождаем неуправляемые объекты             
                _disposed = true;
            }
        }

        /// <summary>
        /// Деструктор
        /// </summary>
        ~ReadOnlyStream()
        {
            Dispose(false);
        }
    }
}