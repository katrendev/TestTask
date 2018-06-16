using System.Collections.Generic;

namespace TestTask
{
    public interface IAlphabetProvider
    {
        IEnumerable<char> Alphabet { get; }

        IEnumerable<char> Vowels { get; }

        IEnumerable<char> Consonants { get; }
    }
}