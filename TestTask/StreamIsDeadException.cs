using System;

namespace TestTask
{
    /// <summary>
    /// Специальное исключение
    /// </summary>
    class StreamIsDeadException : ApplicationException
    {
        private string messageStream;

        public StreamIsDeadException(string messageStream)
        {
            this.messageStream = messageStream;
        }

        /// <summary>
        /// Переопределение метода
        /// </summary>
        public override string Message => $"Stream Error Message : {messageStream}";
    }
}
