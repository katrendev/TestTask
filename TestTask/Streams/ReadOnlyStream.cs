using System;
using System.IO;
using TestTask.Streams.Interfaces;

namespace TestTask.Streams
{
    /// <summary>
    /// Поток для чтения файла.
    /// </summary>
    public class ReadOnlyStream : IReadOnlyStream
    {
        #region Private Fields

        /// <summary>
        /// Поток чтения файла.
        /// </summary>
        private StreamReader _localStream;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            _localStream = new StreamReader(fileFullPath);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsEof => _localStream.EndOfStream;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Dispose()
        {
            _localStream.Close();
            _localStream.Dispose();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public char ReadNextChar()
        {
            if (!IsEof)
            {
                return (char)_localStream.Read();
            }

            throw new Exception("File was ended.");
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void ResetPositionToStart()
        {
            _localStream.DiscardBufferedData();
            _localStream.BaseStream.Seek(0, SeekOrigin.Begin);
        }

        #endregion Public Methods
    }
}