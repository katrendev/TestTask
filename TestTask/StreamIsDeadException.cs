using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask
{
    /// <summary>
    /// Специальное исключение
    /// </summary>
    class StreamIsDeadException : ApplicationException
    {
        private string massageStream;

        public StreamIsDeadException(string massageStream)
        {
            this.massageStream = massageStream;
        }

        /// <summary>
        /// Переопределение метода
        /// </summary>
        public override string Message => $"Stream Error Massege : {massageStream}";
    }
}
