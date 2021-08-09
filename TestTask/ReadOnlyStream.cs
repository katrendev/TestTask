using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private StreamReader _localStream=null;
        private bool _isEof = false;
        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            IsEof = true;
            _localStream = new StreamReader(fileFullPath);
        }
        ~ReadOnlyStream() {
            Close();
        }
        public bool Close() {
            if (_localStream != null) {
                try {
                    _localStream.Close();
                    return true;
                } catch (IOException ex) { }
            }
            return false;
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get { return _isEof = _localStream.EndOfStream; }
            private set {
                _isEof = value;
            }
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            return (char)_localStream.Read();
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
            IsEof = false;
        }
    }
}
