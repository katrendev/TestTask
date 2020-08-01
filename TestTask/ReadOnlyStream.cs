using System;
using System.IO;
using System.Runtime.InteropServices;

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
        public ReadOnlyStream(string fileFullPath)
        {
            // TODO : Заменить на создание реального стрима для чтения файла!
            var fileStream = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read);
            _localStream = new StreamReader(fileStream);
        }
                
        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        
        // TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
        public bool IsEof => _localStream.EndOfStream;

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>

        // TODO : Необходимо считать очередной символ из _localStream
        public char ReadNextChar()
        {
            if (IsEof)
            {
                throw new IOException("Попытка чтения после достижения конца файла");
            }
            return (char) _localStream.Read();
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart() => _localStream.BaseStream.Position = 0;

        public void Dispose() => _localStream.Close();
    }
}
