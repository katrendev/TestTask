using System;

namespace TestTask
{
	/// <summary>
	/// Интерфейс для работы с файлом в сильно урезаном виде.
	/// Умеет всего 2 вещи: прочитать символ, и перемотать стрим на начало.
	/// </summary>
	internal interface IReadOnlyStream : IDisposable
	{
		#region Public Properties

		bool IsEof { get; }

		#endregion Public Properties

		#region Public Methods

		string ReadNextChar();

		void ResetPositionToStart();

		#endregion Public Methods
	}
}