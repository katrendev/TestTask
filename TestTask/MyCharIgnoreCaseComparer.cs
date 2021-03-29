using System.Collections.Generic;

namespace TestTask
{
    class MyCharIgnoreCaseComparer : IEqualityComparer<char>
    {
        public bool Equals(char x, char y)
        {
            return CharExtensions.CompareIgnoreCases(x, y);
        }

        public int GetHashCode(char obj)
        {
            return char.ToUpper(obj).GetHashCode();
        }
    }
}
