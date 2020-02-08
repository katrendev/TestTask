using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private StreamReader _localStream;

        /// <summary>
        /// Конструктор класса. 
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            _localStream = new StreamReader(fileFullPath, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get
            {
                return _localStream.EndOfStream; 
            }
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            if (IsEof)
                throw new Exception("попытка прочитать символ после достижения конца файла");

            return (char)_localStream.Read();            
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            _localStream.BaseStream.Position = 0;          
        }

        public void Dispose()
        {
            _localStream.Dispose();
        }
    }
}
