using System;
using System.IO;
using System.Text;

namespace TestTask
{
    /// <summary>
    /// Представляет обертку над потоком данных только для чтения из входящего файла.
    /// </summary>
    public sealed class ReadOnlyStream : IReadOnlyStream
    {
        #region Private Fields

        /// <summary>
        /// Поток данных полученый из входящего файла.
        /// </summary>
        private readonly StreamReader _localStream;
        StreamReader streamReader;
        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsEndOfFile
        {
            // TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
            get;//=> _localStream.Position == _localStream.Length;
            private set; //{ }
        }

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
                int streamSize = (int)fileStream.Length;
                _localStream = new StreamReader(fileStream, Encoding.UTF8);

                //fileStream.CopyTo(_localStream);
            }
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Dispose()
        {
            //Хотя при использовании MemoryStream, Dispose() можно и не вызывать.
            _localStream?.Dispose();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public char ReadNextChar()
        {
            // TODO : Необходимо считать очередной символ из _localStream
            //_localStream.Position
            throw new NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_localStream == null)
            {
                IsEndOfFile = true;
                return;
            }

            //_localStream.Position = 0;
            IsEndOfFile = false;
        }

        #endregion Public Methods
    }
}
