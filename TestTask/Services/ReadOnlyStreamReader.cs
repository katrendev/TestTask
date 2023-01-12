using System;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Text;
using TestTask.Services.Interfaces;

namespace TestTask.Services
{
    /// <summary>
    /// Работа с файлами реализована через FileStream
    /// </summary>
    public class ReadOnlyStreamReader : IReadOnlyStream
    {
        /// <summary>
        /// Поток для чтения
        /// </summary>
        private StreamReader _localStream;

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get => _localStream?.EndOfStream ?? true;
        }
        /// <summary>
        /// Конструктор класса. 
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStreamReader(string fileFullPath)
        {
            try
            {
                _localStream = new StreamReader(fileFullPath);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            if (_localStream == null) throw new NullReferenceException();
            if (_localStream.BaseStream == null) throw new ObjectDisposedException("Чтение из закрытого TextReader невозможно");
            if (IsEof) throw new EndOfStreamException();

            return (char)_localStream.Read();
        }   

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_localStream == null) throw new NullReferenceException();
            if (_localStream.BaseStream == null) throw new ObjectDisposedException("Чтение из закрытого TextReader невозможно");
            if (_localStream.BaseStream.Position == 0) return;

            //сбрасываем внутренний буфер
            _localStream.DiscardBufferedData();
            _localStream.BaseStream.Seek(0L,SeekOrigin.Begin);

            //текстовые редакторы по типу блокнота добавляют неразрывный пробел 0 ширины, прочитаем его
            if (_localStream.Peek() == 65279)
                _localStream.Read();
        }

        /// <summary>
        /// Освобождение ресурсов (закрываем поток)
        /// </summary>
        public void Dispose() => _localStream?.Dispose();
    }
}
