using System;
using System.IO;
using System.Text;
using TestTask.Streams.Interfaces;

namespace TestTask.Streams
{
	/// <summary>
	/// Кастомный класс потока для чтения текстовых файлов.
	/// </summary>
	public class ReadOnlyStream : IReadOnlyStream
	{
		#region Fields

		/// <summary>
		/// Поток для чтения файла.
		/// </summary>
		private StreamReader _streamReader;

		#endregion Fields

		#region Properties

		/// <inheritdoc/>
		public bool IsEof
		{
			get;
			private set;
		}

		#endregion Properties

		#region Public Constructors

		/// <summary>
		/// Конструктор класса.
		/// </summary>
		/// <param name="fileFullPath">Полный путь до файла для чтения</param>
		public ReadOnlyStream(string fileFullPath) : this(fileFullPath, Encoding.UTF8)
		{
		}

		/// <summary>
		/// Конструктор класса.
		/// </summary>
		/// <param name="fileFullPath">Полный путь до файла для чтения.</param>
		/// <param name="encoding">Кодировка.</param>
		public ReadOnlyStream(string fileFullPath, Encoding encoding)
		{
			var localStream = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read);
			_streamReader = new StreamReader(localStream, encoding);
		}

		#endregion Public Constructors

		#region Public Methods

		/// <inheritdoc/>
		public void Dispose()
		{
			_streamReader?.Dispose();
			_streamReader = null;
		}

		/// <inheritdoc/>
		public char ReadNextChar()
		{
			if (IsEof)
			{
				throw new EndOfStreamException("Невозможно прочитать текст после конца потока.");
			}

			char letter = (char)_streamReader.Read();

			if (_streamReader.EndOfStream)
				IsEof = true;

			return letter;
		}

		/// <inheritdoc/>
		public void ResetPositionToStart()
		{
			if (_streamReader != null)
			{
				_streamReader.DiscardBufferedData();
				_streamReader.BaseStream.Position = 0;
			}

			IsEof = false;
		}

		#endregion Public Methods
	}
}