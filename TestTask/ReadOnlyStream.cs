using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private StreamReader _localStream;

        char[] buffer;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            buffer = new char[1];
            // TODO : Заменить на создание реального стрима для чтения файла!
            try
            {
                _localStream = new StreamReader(fileFullPath);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message + "\n");//Добавим пустую строку для читабельности
                return;
            }
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get  // TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
            {
                if (_localStream != null && !disposedValue)
                {
                    return _localStream.EndOfStream;
                }
                return true;
            }
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
            if (IsEof)
            {
                throw new Exception("Файл уже прочитан до конца");
            }
            int charsReaded = _localStream.ReadBlock(buffer, 0, 1);
            if (charsReaded == 0) //Дочитали до конца потока, но символа так и не получилось
            {
                throw new Exception("Файл прочитан до конца, но считанные данные не являются символом");
            }
            return buffer[0];
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_localStream == null)
            {
                return;
            }

            _localStream.BaseStream.Position = 0;
        }

        #region IDisposable Support, стандартная реализация

        private bool disposedValue = false; // Для определения избыточных вызовов

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if(_localStream!=null)
                    {
                        _localStream.Dispose();
                    }
                    //освободить управляемое состояние (управляемые объекты).
                }

                // освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // задать большим полям значение NULL.

                disposedValue = true;
            }
        }

        //// переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        //~ReadOnlyStream()
        //{
        //    // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
        //    Dispose(false);
        //}

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
            // раскомментировать следующую строку, если метод завершения переопределен выше.
            //GC.SuppressFinalize(this);
        }

        #endregion
    }
}
