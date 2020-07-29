using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private Stream _localStream;
        private StreamReader _localStreamReader;
        private string fileFullPath;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
             this.fileFullPath = fileFullPath;
            _localStream = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read);
            _localStreamReader = new StreamReader(_localStream);
        }
                
        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof => _localStreamReader.EndOfStream;

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            if (IsEof)
                throw new IndexOutOfRangeException();

            var buf = _localStreamReader.Read();
            return (char)buf;
        }

        /// <summary>
        /// Закрывает прочитанный поток и открывает новый
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_localStream == null)
            {
                return;
            }
            _localStreamReader.Close(); // _localStream также закрывается
            _localStream = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read);
            _localStreamReader = new StreamReader(_localStream);
        }
        
        /// <summary>
        /// Освобождение неуправляемых ресурсов
        /// </summary>
        public void Dispose() 
        {
            _localStreamReader.Dispose();
            _localStream.Dispose();
        }

        /// <summary>
        /// Гарантированное закрытие потока
        /// </summary>
        public void Close()
        {
            _localStreamReader.Close();
            _localStream.Close();
            Dispose();
        }
    }
}
