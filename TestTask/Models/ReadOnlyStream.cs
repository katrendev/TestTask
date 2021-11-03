using System;
using System.IO;
using System.Text;
using TestTask.Interfaces;

namespace TestTask.Models
{
    /// <summary>
    /// Представляет обертку над потоком данных только для чтения из входящего файла.
    /// </summary>
    public sealed class ReadOnlyStream : IReadOnlyStream
    {
        #region Private Fields

        /// <summary>
        /// Поток данных полученый из входящего файла который хранится в памяти.
        /// </summary>
        private readonly MemoryStream _localStream;

        /// <summary>
        /// Преобразованный поток данных из файла для удобной работы с текстами.
        /// </summary>
        private readonly StreamReader _streamReader;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsEndOfFile => _streamReader.EndOfStream;

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            // TODO : Заменить на создание реального стрима для чтения файла!
            using (FileStream fileStream = File.OpenRead(fileFullPath))
            {
                var streamCapacity = (int)fileStream.Length;
                _localStream = new MemoryStream(streamCapacity);

                fileStream.CopyTo(_localStream);

                _streamReader = new StreamReader(_localStream, Encoding.UTF8);
            }
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Dispose()
        {
            _localStream?.Dispose();
            _streamReader?.Dispose();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public char ReadNextChar()
        {
            // TODO : Необходимо считать очередной символ из _localStream
            try
            {
                var simbol = _streamReader.Read();
                return Convert.ToChar(simbol);
            }
            catch(OverflowException exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void ResetPositionToStart()
        {
            _streamReader.BaseStream.Position = 0;
        }

        #endregion Public Methods
    }
}
