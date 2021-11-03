using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask.Helpers
{
    internal static class CharTypeHelper
    {
        /// <summary>
        /// Набор гласных букв для русского языка.
        /// </summary>
        internal const string RUS_EN_VOWELS = "АУОЫИЭЯЮЁЕAEIOUY";

        /// <summary>
        /// Набор согласных букв для русского языка.
        /// </summary>
        internal const string RUS_EN_CONSONANTS = "БВГДЖЗЙКЛМНПРСТФХЦЧШЩBCDFGHJKLMNPQRSTVWXZ";

        /// <summary>
        /// Набор гласных букв для английского языка.
        /// </summary>
        internal const string EN_VOWELS = "AEIOUY";

        /// <summary>
        /// Набор согласных букв для английского языка.
        /// </summary>
        internal const string EN_CONSONANTS = "BCDFGHJKLMNPQRSTVWXZ";
    }
}
