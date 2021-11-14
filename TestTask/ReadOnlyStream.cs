using System;
using System.IO;
using System.Text;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private Stream _localStream;
        private event Action EndOfFile;
        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            IsEof = true;
            EndOfFile += CloseStream;
            // TODO : Заменить на создание реального стрима для чтения файла!
            try
            {
                _localStream = File.Open(fileFullPath, FileMode.Open);
            }
            catch
            {
                Console.WriteLine("Не удалось открыть файл");
                _localStream = null;
            }
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get;// TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
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
            byte[] bytes = new byte[2];
            _localStream.Read(bytes, 0, 2);
            char[] nextChar = new char[1];
            Encoding.Unicode.GetChars(bytes, 0, 2, nextChar, 0);
            UpdateIsEof();
            return nextChar[0];
        }

        private void UpdateIsEof()
        {
            IsEof = _localStream.Length == _localStream.Position;
            if (IsEof == true)
                EndOfFile?.Invoke();
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
            UpdateIsEof();
        }
        private void CloseStream()
        {
            _localStream.Close();
        }
    }
}
