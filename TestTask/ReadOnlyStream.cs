using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private StreamReader _reader;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            // TODO : Заменить на создание реального стрима для чтения файла!
            if (File.Exists(fileFullPath))
                _reader = new StreamReader(fileFullPath, System.Text.Encoding.Default);
            else
                throw new FileNotFoundException("File not Found!", fileFullPath);
        }

        ~ReadOnlyStream()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_reader != null)
            {
                _reader.Close();
                _reader = null;
            }
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get { return _reader.EndOfStream; } // TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
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
            if (!_reader.EndOfStream)
                try
                {
                    return (char)_reader.Read();
                }
                catch (Exception ex)
                {
                    Dispose();
                    throw ex;
                }
            else
            {
                Dispose();
                throw new Exception("End of File is reached!");
            }
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_reader == null) return;

            _reader.BaseStream.Position = 0;
        }
    }
}