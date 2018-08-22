using System;
using System.IO;
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
        /// <param name="encoding">Кодировка (по умолчанию - автоопределение или UTF-8)</param>
        public ReadOnlyStream(string fileFullPath,Encoding encoding=null)
        {
            IsEof = true;
            try
            {
                _localStream = (encoding != null) ? new StreamReader(fileFullPath, encoding)
                    : new StreamReader(fileFullPath, true);
            }
            catch (Exception e)
            {
                throw new Exception($"Ошибка открытия файла '{fileFullPath}'", e);
            }
            
            // TODO : Заменить на создание реального стрима для чтения файла!            
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
            var c=_localStream.Read(); 
            if (_localStream.Peek() == -1)
                IsEof = true;            
            return (char)c;            
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

            _localStream.BaseStream.Seek(0, SeekOrigin.Begin);
            IsEof = false;
        }
   
        private bool disposed = false;
        // реализация интерфейса IDisposable.
        public void Dispose()
        {
            CleanUp(true);
            GC.SuppressFinalize(this); // подавляем финализацию
        }

        protected void CleanUp(bool disposing)
        {
            if (!disposed)
            {
                if (disposing) // освобождение управляемых ресурсов
                    _localStream.Close();
                disposed = true;
            }
        }

        // Деструктор
        ~ReadOnlyStream()
        {
            CleanUp(false);
        }
    }
}
