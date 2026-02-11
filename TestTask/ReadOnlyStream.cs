using System;
using System.IO;
using System.Text;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private const string __errorNoMoreSymbols = "Нет символов для чтения";
        private const string __errorStreamSeekNotSupport = "Файловый поток не поддерживает перемещение";
        private const string __errorFileOpen = "Ошибка открытия файла '{0}'.";
        private const string __errorNullStream = "Ошибка создания потока.";

        private readonly Stream _localStream;
        private bool _disposed = false;

        /// <summary>
        /// Кодировка для чтения символа из файла.
        /// </summary>
        public Encoding Encoding { get; }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        // TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении (+)
        public bool IsEof => _localStream.Position >= _localStream.Length;

        /// <summary>
        /// Конструктор класса. 
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath, Encoding encoding = default)
        {
            Encoding = DefineEncoding(encoding);
            // TODO : Заменить на создание реального стрима для чтения файла! (+)
            _localStream = CreateStream(fileFullPath);
            CheckStream();
        }

        /// <summary>
        /// Метод открывает Stream по имени файла (можно переопределять).
        /// </summary>
        /// <param name="param">Полный путь до файла для чтения</param>
        /// <returns>Открытый поток.</returns>
        public virtual Stream CreateStream(string param)
        {
            CheckPath(param);
            return TryOpenStream(param);
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            CheckDisposed();
            // TODO : Необходимо считать очередной символ из _localStream (+)
            byte[] buffer = new byte[sizeof(char)];
            int bytesRead = _localStream.Read(buffer, 0, sizeof(char));
            if (bytesRead != sizeof(char))
                throw new EndOfStreamException(__errorNoMoreSymbols);
            return Encoding.GetChars(buffer)[0];
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            if (!_localStream.CanSeek)
                throw new InvalidOperationException(__errorStreamSeekNotSupport); 

            _localStream.Position = 0;
        }

        /// <summary>
        /// Метод для очистки неуправляемых ресурсов.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _localStream.Dispose();
                _disposed = true;
            }
        }

        /// <summary>
        /// Метод выбрасывает исключение, если ресурс уже освобожден.
        /// </summary>
        private void CheckDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        /// <summary>
        /// Метод проверяет внутренний стрим на null, выбрасывает исключение.
        /// </summary>
        private void CheckStream()
        {
            if (_localStream == null || _localStream == Stream.Null)
                throw new ArgumentNullException(nameof(Stream), __errorNullStream);
        }

        /// <summary>
        /// Метод пытается открыть файловый поток, выбрасывает исключение при невозможности.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения.</param>
        /// <returns>Открытый файловый поток.</returns>
        private static Stream TryOpenStream(string fileFullPath)
        {
            try
            {
                return File.OpenRead(fileFullPath);
            }
            catch (IOException ex)
            {
                throw new IOException(string.Format(__errorFileOpen, fileFullPath), ex);
            }
        }

        /// <summary>
        /// Метод проверяет путь к файлу на null и пустоту.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла</param>
        private static void CheckPath(string fileFullPath)
        {
            if (string.IsNullOrWhiteSpace(fileFullPath))
                throw new ArgumentNullException(nameof(fileFullPath));
        }

        /// <summary>
        /// Метод проверяет Encoding на null, возвращает UTF8 при положительной проверке.
        /// </summary>
        /// <param name="encoding">Задаваемая кодировка</param>
        /// <returns>Кодировка Encoding.</returns>
        private static Encoding DefineEncoding(Encoding encoding) =>
            encoding == default
                ? Encoding.UTF8
                : encoding;
    }
}
