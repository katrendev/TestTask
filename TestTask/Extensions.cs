using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask
{
    internal static class Extensions
    {
        public static bool IsVowel(this char @char)
        {
            return "aeiouаяуюоеёэиы".IndexOf(@char.ToString(), StringComparison.InvariantCultureIgnoreCase) >= 0;
        }
    }
}
