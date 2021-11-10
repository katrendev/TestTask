using System;
using System.IO;
using System.Text;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private StreamReader _localStream;
        private bool disposed = false;
        private bool _IsEof = false;
        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            try
            {
                _localStream = new StreamReader(fileFullPath, Encoding.UTF8);
            }

            catch (Exception e)
            {
                System.Console.WriteLine("Не удалось открыть файл! Возникла ошибка: {0}" + e.ToString());
            }

            // TODO : Заменить на создание реального стрима для чтения файла!
        }
        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get
            {
                return _IsEof;
            }// TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
            private set
            {
                if (_localStream.EndOfStream)
                {
                    _IsEof = true;
                }
                else
                {
                    _IsEof = value;
                }
            }
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        /// 
        public char ReadNextChar()
        {
            try
            {
                int nextChar = _localStream.Read();
                if (nextChar > 0)
                {
                    return (char)nextChar;
                }
                else
                {
                    _IsEof = true;
                    Dispose();
                    throw new EndOfStreamException();
                }
            }
            catch (Exception e)
            {
                throw e;

            }
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            _localStream.BaseStream.Position = 0;
        }

        public void Dispose()
        {
            DisposeStream();
        }
        public void DisposeStream()
        {
            _localStream.Close();
            disposed = true;
        }

        public string ReadToEnd()
        {
            return _localStream.ReadToEnd();
        }

        ~ReadOnlyStream()
        {
            if (!disposed)
            {
                Dispose();
            }
        }
    }
}
