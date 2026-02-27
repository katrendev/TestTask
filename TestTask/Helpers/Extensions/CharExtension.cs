namespace TestTask.Helpers.Extensions
{
	/// <summary>
	/// Расширение для char.
	/// </summary>
	internal static class CharExtension
	{
		#region Public Methods

		/// <summary>
		/// Проверяет символ английская ли глассная буква?
		/// </summary>
		/// <param name="value">Входной символ.</param>
		/// <returns>Английская глассная буква?</returns>
		public static bool IsEnglishVowel(this char value)
		{
			value = char.ToLowerInvariant(value);

			switch (value)
			{
				case 'a':
				case 'e':
				case 'i':
				case 'o':
				case 'u':
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// Проверяет символ русская ли глассная буква?
		/// </summary>
		/// <param name="value">Входной символ.</param>
		/// <returns>Русская глассная буква?</returns>
		public static bool IsRussianVowel(this char value)
		{
			value = char.ToLowerInvariant(value);

			switch (value)
			{
				case 'а':
				case 'е':
				case 'ё':
				case 'и':
				case 'о':
				case 'у':
				case 'ы':
				case 'э':
				case 'ю':
				case 'я':
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// Проверяет символ глассная ли буква?
		/// </summary>
		/// <param name="value">Входной символ.</param>
		/// <returns>Глассная буква?</returns>
		/// <remarks>Проверка русского и английского алфовита.</remarks>
		public static bool IsVowel(this char value)
		{
			return IsRussianVowel(value) || IsEnglishVowel(value);
		}

		#endregion Public Methods
	}
}