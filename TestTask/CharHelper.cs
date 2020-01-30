using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TestTask
{
	public static class CharHelper
	{
		public static readonly ReadOnlyDictionary<char, CharType> CharTypes = new ReadOnlyDictionary<char, CharType>(new Dictionary<char, CharType>()
		{
			{'a', CharType.Vowel},
			{'у', CharType.Vowel},
			{'о', CharType.Vowel},
			{'ы', CharType.Vowel},
			{'и', CharType.Vowel},
			{'э', CharType.Vowel},
			{'я', CharType.Vowel},
			{'ю', CharType.Vowel},
			{'ё', CharType.Vowel},
			{'е', CharType.Vowel}
		});
	}
}