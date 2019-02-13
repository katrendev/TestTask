using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        //private Stream _localStream;
        private StreamReader _localStream;
        private int _currentCharacter = -1;
        private bool _isEof = true;
        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            _localStream = new StreamReader(File.Open(fileFullPath, FileMode.Open));
        }
                
        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get { return _isEof; }
            private set { _isEof = value; }
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
            _currentCharacter = _localStream.Read();
            _isEof = _localStream.EndOfStream;
            if (_currentCharacter != -1)
            {
                return (char)_currentCharacter;
            }

            throw new EndOfStreamException();
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
            
            //TODO: сбросить позиция на начало            
            _localStream.BaseStream.Position = 0;
            _localStream.BaseStream.Seek(0, SeekOrigin.Begin);
            IsEof = _localStream.Peek() == -1;
        }

        public void Dispose()
        {
            _localStream.Dispose();
        }
    }
}
