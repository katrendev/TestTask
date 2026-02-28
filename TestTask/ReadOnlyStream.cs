using System;
using System.IO;
using System.Runtime.Remoting.Messaging;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private StreamReader _localStream;
        private readonly string _filePath;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            _filePath = fileFullPath;
            // Инициализируем стрим сразу
            ResetPositionToStart();
        }
                
        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get;
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
            if (IsEof) throw new EndOfStreamException("Достигнут конец файла");

            int charCode = _localStream.Read();
            if (charCode == -1)
            {
                IsEof = true;
                return '\0';
            }

            return (char)charCode;
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            _localStream?.Dispose();
            _localStream = new StreamReader(_filePath);
            IsEof = false;
        }

        /// <summary>
        /// Освобождаем память
        /// </summary>
        public void Dispose()
        {
            _localStream?.Dispose();
        }
    }
}
