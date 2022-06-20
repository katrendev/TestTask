using System.Collections.Generic;

namespace TestTask.Data.English
{
    /// <summary>
    /// Содержит список букв каждого типа: гласные и согласные.
    /// </summary>
    internal struct ListOfCharsTypes
    {
        #region Public Fields

        /// <summary>
        /// Список согласных букв.
        /// </summary>
        public static readonly IReadOnlyList<char> ConsonantsChars = new List<char>()
        {
            'B','b',

            'C','c',

            'D', 'd',

            'F', 'f',

            'G','g',

            'H','h',

            'J','j',

            'K','k',

            'L','l',

            'M','m',

            'N','n',

            'P','p',

            'Q','q',

            'R','r',

            'S','s',

            'T', 't',

            'V','v',

            'W','w',

            'X','x',

            'Z','z'
        };

        /// <summary>
        /// Список гласных букв.
        /// </summary>
        public static readonly IReadOnlyList<char> VovelChars = new List<char>()
        {
            'A', 'a',
            'E','e',
            'I', 'i',
            'O','o',
            'U','u',
            'Y','y'
        };

        #endregion Public Fields
    }
}