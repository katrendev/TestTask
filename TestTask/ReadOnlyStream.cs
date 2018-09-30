using System;
using System.IO;
using System.Text;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private readonly StreamReader _localStream;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            IsEof = true;

            _localStream = new StreamReader(fileFullPath, Encoding.Default);
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get;
            set;
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            //Проверка на попытку считать символ после закрытия потока
            if (IsEof)
            {
                throw new ArgumentOutOfRangeException("Чтение файла завершено.");
            }

            var nextChar = (char)_localStream.Read();

            //Проверка на окончание файла
            if (nextChar == '\uffff')
            {
                IsEof = true;
                _localStream.Close();
            }

            return nextChar;
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            _localStream.DiscardBufferedData();
            _localStream.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);

            IsEof = false;
        }
    }
}