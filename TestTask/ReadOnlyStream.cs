using System;
using System.IO;
using System.Text;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private Stream _localStream;

        private bool _isEof;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            IsEof = true;


            _localStream = new FileInfo(fileFullPath).OpenRead();
            //_localStream = File.OpenText(fileFullPath);


            // TODO : Заменить на создание реального стрима для чтения файла!
            //_localStream = null;
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
            int b = _localStream.ReadByte();

            //Если последний символ, IsEof присвоить true
            if (_localStream.Position == _localStream.Length) IsEof = true;

            //Если конец файла, то сгенерировать специальное исключение
            try
            {
                return b == -1 ?
                    throw new StreamIsDeadException("Попытка прочитать сомвол после достижения конца файла!!!")
                    : Encoding.Default.GetChars(new byte[] { (byte)b })[0];
            }
            catch(StreamIsDeadException e)
            {
                Console.WriteLine(e.Message);
                return default;
            }

            // TODO : Необходимо считать очередной символ из _localStream
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

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public void Dispose()
        {
            _localStream.Dispose();
        }
    }
}
