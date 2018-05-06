using System;
using System.IO;
using System.Text;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        bool isDisposed = false;
        BinaryReader binaryReader;
        private Stream _localStream;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <param name="encoding">Кодировка файла</param>
        public ReadOnlyStream(string fileFullPath, Encoding encoding)
        {
            if (string.IsNullOrWhiteSpace(fileFullPath))
            {
                throw new ArgumentNullException(nameof(fileFullPath));
            }
            this.Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));

            if (!File.Exists(fileFullPath))
            {
                throw new FileNotFoundException("Не найден файл");
            }

            // TODO : Заменить на создание реального стрима для чтения файла!
            _localStream = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read);
            this.binaryReader = new BinaryReader(_localStream, encoding);
        }
        /// <summary>
        /// Конструктор класса. Для чтения файла в кодировке UTF8
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath) : this(fileFullPath, Encoding.UTF8) { }

        public Encoding Encoding { get; }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof => _localStream.Length == _localStream.Position;

        public virtual void Close()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            this.Close();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                this.binaryReader.Dispose();
            }

            this.isDisposed = true;
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
            return this.binaryReader.ReadChar();
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            _localStream.Position = 0;
        }
    }
}
