using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private readonly StreamReader _localStream;//поскольку работа с текстовым файлом изменим тип потока для дальнейшего чтения символов, а не потока байт
        private bool disposed = false;
        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {

            // TODO : Заменить на создание реального стрима для чтения файла!

            try
            {
                _localStream = new StreamReader(fileFullPath);
            }
            catch(FileNotFoundException)
            {
                Console.WriteLine("The file or directory cannot be found.");
            }
        }
                
        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
          // TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
            get { if (_localStream == null || _localStream.EndOfStream)
                         return true; 
                    return false; }

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

            _localStream.BaseStream.Seek(0, SeekOrigin.Begin);

            _localStream.DiscardBufferedData();
        }
        //следующий два метода реализуют интерфейс IDisposable для освобождения неуправляемых ресурсов
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _localStream.Dispose();
                }
                disposed = true;
            }

        }


    }
}
