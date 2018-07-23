using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream, IDisposable
    {
        private Stream _localStream;
        private TextReader _localReader;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            IsEof = true;

            // TODO : Заменить на создание реального стрима для чтения файла!
            // В конструкторе инициализируем файловый и текстовый поток.
            if (String.IsNullOrEmpty(fileFullPath))
                throw new ArgumentNullException("fileFullPath");
            try
            {
                _localStream = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                _localReader = new StreamReader(_localStream);
                IsEof = false;
            }
            catch
            {
                Dispose();
                _localStream = null;
            }
        }
                
        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get; // TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
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
            // Реализуем чтение одного символа из текстового потока.
            // После достижения коца файла закрываем потоки.
            int iCh;
            char ch;

            try
            {
                iCh = _localReader.Read();
                ch = (char)iCh;
                if ((iCh = _localReader.Peek()) == -1)
                {
                    IsEof = true;
                    Dispose();
                }
            }
            catch
            {
                IsEof = true;
                Dispose();
                ch = Char.Parse("1");
            }
            return ch;

            //throw new NotImplementedException();
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

        // Метод который гарантирует закрытие потоков.
        public void Dispose()
        {
            if (_localReader != null)
            {
                _localReader.Close();
                _localReader.Dispose();
            }
            if (_localStream != null)
            {
                _localStream.Close();
                _localStream.Dispose();
            }
        }
    }
}
