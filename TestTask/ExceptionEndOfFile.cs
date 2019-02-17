using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask
{
    /// <summary>
    /// Ошибка достигнут конец файла
    /// </summary>
    public class ExceptionEndOfFile : Exception
    {
        private readonly string _message;

        public ExceptionEndOfFile()
        {
            _message = "Достигнут конец файла";
        }

        new public string Message
        {
            get { return _message; }
        }
    }
}
