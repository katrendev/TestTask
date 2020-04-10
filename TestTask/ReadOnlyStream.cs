using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private Stream _localStream;
        BinaryReader BW;
        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            _IsEof = true;


            _localStream = File.OpenRead(fileFullPath);

            BW = new BinaryReader(_localStream);
            _IsEof = false;
        }

        bool _IsEof;
        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get { return _IsEof || _localStream.Position == _localStream.Length; }
            private set { _IsEof = value; }
        }

        public void Close()
        {
            _localStream.Close();
            _localStream.Dispose();
        }

        public void Dispose()
        {
            try
            {
                Close();
            }
            catch {}
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            try
            {
                return BW.ReadChar();
            }
            catch (Exception)
            {
                _IsEof = true;
                throw;
            }
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_localStream == null)
            {
                _IsEof = true;
                return;
            }

            _localStream.Position = 0;
            _IsEof = false;
        }
    }
}
