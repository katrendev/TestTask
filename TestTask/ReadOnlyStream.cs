using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
		private StreamReader _localStream;
	    private bool _disposed = false;

		/// <summary>
		/// Конструктор класса. 
		/// Т.к. происходит прямая работа с файлом, необходимо 
		/// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
		/// </summary>
		/// <param name="fileFullPath">Полный путь до файла для чтения</param>
		public ReadOnlyStream(string fileFullPath)
        {
	        _localStream = new StreamReader(fileFullPath);
	        ResetPositionToStart();
		}

	    /// <summary>
	    /// Флаг окончания файла.
	    /// </summary>
	    public bool IsEof { get; private set; }

	    /// <summary>
	    /// Ф-ция чтения следующего символа из потока.
	    /// Если произведена попытка прочитать символ после достижения конца файла, метод 
	    /// должен бросать соответствующее исключение
	    /// </summary>
	    /// <returns>Считанный символ.</returns>
	    public char ReadNextChar()
	    {
		    if (_localStream.EndOfStream)
			    throw new EndOfStreamException();

		    var nextChar = (char)_localStream.Read();

		    if (_localStream.EndOfStream)
			    IsEof = true;

		    return nextChar;
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

		    _localStream.BaseStream.Position = 0;
		    IsEof = false;
	    }

	    /// <summary>
	    /// Позволяет получить значение позиции
	    /// </summary>
	    /// <returns>Текущая позиция</returns>
	    public long GetPosition()
	    {
		    if (_localStream == null)
			    throw new Exception("Stream is null");

		    return _localStream.BaseStream.Position;
	    }

	    /// <summary>
	    /// Освобождает все ресурсы, используемые <see cref="ReadOnlyStream" />
	    /// </summary>
	    /// <inheritdoc cref="IDisposable"/>
	    public void Dispose()
	    {
		    Dispose(true);
		    GC.SuppressFinalize(this);
	    }

	    protected virtual void Dispose(bool disposing)
	    {
		    if (!_disposed)
		    {
			    if (disposing)
			    {
				    _localStream?.Dispose();
				    _localStream = null;
			    }
			    _disposed = true;
		    }
	    }
	}
}
