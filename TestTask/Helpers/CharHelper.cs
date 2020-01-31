using System.Collections.Generic;
using System.Collections.ObjectModel;
using TestTask.Models;

namespace TestTask.Helpers
{
	public static class CharHelper
	{
		public static readonly ReadOnlyDictionary<char, CharType> Vowels = new ReadOnlyDictionary<char, CharType>(new Dictionary<char, CharType>()
		{
			{'А', CharType.Vowel},
			{'У', CharType.Vowel},
			{'О', CharType.Vowel},
			{'Ы', CharType.Vowel},
			{'И', CharType.Vowel},
			{'Э', CharType.Vowel},
			{'Я', CharType.Vowel},
			{'Ю', CharType.Vowel},
			{'Ё', CharType.Vowel},
			{'Е', CharType.Vowel}
		});
	}
}