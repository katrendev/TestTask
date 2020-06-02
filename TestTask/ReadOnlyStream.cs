using System;
using System.IO;
using System.Text;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private bool isEof;
        private Stream _localStream;
        private StreamReader _reader;
        public long LenghtFile { get; private set; }
        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            // TODO : Заменить на создание реального стрима для чтения файла! 
            if (File.Exists(fileFullPath))
            {
                _localStream = File.OpenRead(fileFullPath);
                _reader = new StreamReader(_localStream, Encoding.GetEncoding(65001));
                IsEof = false;
                _localStream.Position = 0;
                LenghtFile = _localStream.Length;
                //_localStream = null;  
            }
            else
            {
                throw new NotImplementedException(Title.ErrorMessage);
            }
        }
        public void Dispose()
        {
            _localStream.Dispose();
        }
        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get // TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
            {
                return isEof;
            }
            private set
            {
                if (_localStream.Position >= _localStream.Length)
                    isEof = value;
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
            if (_reader.Peek() >= 0)
            {
                return (char)_reader.Read();
            }
            else
            {
                IsEof = true;
                throw new NotImplementedException("Достигнут конец файла");
            }
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
            _localStream.Position = 0;
            IsEof = false;
        }
    }
}
