using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestTask
{
    public static class CharExtensions
    {
        public static bool IsCyrillic(this char c)
        {
            return  char.IsLetter(c) && Regex.IsMatch(c.ToString(), @"\p{IsCyrillic}");
        }
    }
}
