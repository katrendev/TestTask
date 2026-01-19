using System;
using System.IO;
using System.Text;

namespace TestTask
{
    /// <summary>
    /// Класс для работы с потоком чтения из текстового файла
    /// </summary>
    public class ReadOnlyStream : IReadOnlyStream
    {
        private StreamReader _localStreamReader;

        /// <summary>
        /// Конструктор класса <see cref="ReadOnlyStream"/>.
        /// Должен обеспечивать ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с ним.
        /// </summary>
        /// <param name="fileFullPath">полный путь к файлу для чтения</param>
        public ReadOnlyStream(string fileFullPath) : this(fileFullPath, Encoding.Default) { }

        /// <summary>
        /// Конструктор класса <see cref="ReadOnlyStream" /> с возможностью указать кодировку файла.
        /// Должен обеспечивать ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с ним.
        /// </summary>
        /// <param name="fileFullPath">полный путь к текстовому файлу для чтения</param>
        /// <param name="encoding">кодировка текстового файла</param>
        /// <exception cref="ArgumentNullException">Source file path can't be NULL or empty</exception>
        /// <exception cref="ArgumentException">Source file doesn't exist"</exception>
        public ReadOnlyStream(string fileFullPath, Encoding encoding)
        {
            if (string.IsNullOrEmpty(fileFullPath))
            {
                throw new ArgumentNullException(nameof(fileFullPath), "Source file path can't be NULL or empty");
            }
            if (!File.Exists(fileFullPath))
            {
                throw new ArgumentException($"Source file \"{fileFullPath}\" doesn't exist", nameof(fileFullPath));
            }
            this._localStreamReader = new StreamReader(fileFullPath, encoding);
        }

        /// <summary>
        /// Флаг окончания файла
        /// </summary>
        public bool IsEof
        {
            get { return this._localStreamReader.EndOfStream; }
        }

        /// <summary>
        /// Читает следующий символ из потока.
        /// Выбрасыает <see cref="T:System.IO.IOException"/>, если произведена попытка прочитать символ после достижения конца файла.
        /// </summary>
        /// <returns>считанный символ</returns>
        public char ReadNextChar()
        {
            return (char)this._localStreamReader.Read();
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            this._localStreamReader.BaseStream.Position = 0;
        }

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this._localStreamReader?.Dispose();
        }

        #endregion
    }
}
