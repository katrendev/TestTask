using System;
using System.IO;
using System.Threading.Tasks;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        #region Constants

        // The task states to read one symbol from stream.
        private const int BUFFER_SIZE = 1;

        #endregion

        #region Private Fields

        private TextReader _localStream;
        private char[] _buffer = new char[BUFFER_SIZE];
        private bool _disposed = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream()
        {
            IsEof = true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get; // TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
            private set;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        internal static async Task<IReadOnlyStream> GetInputStreamAsync(string fileFullPath)
        {
            // TODO : Заменить на создание реального стрима для чтения файла!
            //_localStream = null;

            var instance = new ReadOnlyStream();

            try
            {
                instance._localStream = await InitializeStreamReaderAsync(fileFullPath);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }

            return instance;
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// This method returns a default <see cref="char"/> value if the stream is exhausted.
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public async Task<char> ReadNextCharAsync()
        {
            // TODO : Необходимо считать очередной символ из _localStream
            //throw new NotImplementedException();

            if (IsEof)
            {
                throw new EndOfStreamException("The end of stream has already been reached.");
            }

            int bytesRead = 0;

            try
            {
                bytesRead = await _localStream.ReadAsync(_buffer, 0, BUFFER_SIZE);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }

            if (bytesRead == 0)
            {
                IsEof = true;
            }

            return bytesRead > 0 ? _buffer[0] : default;
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public async Task ResetPositionToStartAsync()
        {
            if (_localStream == null)
            {
                IsEof = true;
            }
            else
            {
                try
                {
                    await Task.Run(() => (_localStream as StreamReader).BaseStream.Position = 0);
                }
                catch (Exception e)
                {
                    throw new Exception(e.ToString());
                }

                IsEof = false;
            }
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            // Release any managed resources here.
            if (disposing)
            {
                _localStream?.Dispose();
            }

            // Dispose of any unmanaged resources not wrapped in safe handles.

            _disposed = true;
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Instantiates and returns a new <see cref="StreamReader"/> object.
        /// </summary>
        /// <param name="path">A full path to a file.</param>
        /// <returns></returns>
        private static Task<StreamReader> InitializeStreamReaderAsync(string path) =>
            Task.Run(() => new StreamReader(path));

        #endregion
    }
}
