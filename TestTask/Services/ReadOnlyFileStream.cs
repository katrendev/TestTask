using System;
using System.IO;
using TestTask.Services.Interfaces;

namespace TestTask.Services
{
    /// <summary>
    /// Работа с файлами реализована через FileStream
    /// </summary>
    internal class ReadOnlyFileStream : IReadOnlyStream
    {
        /// <summary>
        /// Поток для чтения
        /// </summary>
        private Stream _localStream;

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get;
            private set;
        }
        /// <summary>
        /// Конструктор класса. 
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyFileStream(string fileFullPath)
        {
            if(File.Exists(fileFullPath)) _localStream = new FileStream(fileFullPath, FileMode.Open);
            else IsEof = true;            
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            if (IsEof) throw new ArgumentOutOfRangeException(nameof(IsEof),"Достигнут конец файла");

            var bytes = new byte[1];

            if (_localStream.Read(bytes, 0, bytes.Length) == 0) IsEof = true;

            return (char)bytes[0];
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
        /// Освобождение ресурсов (закрываем поток)
        /// </summary>
        public void Dispose()
        {
            _localStream.Dispose();
        }
    }
}
