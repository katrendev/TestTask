using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask
{
    public class AlphabetSingleLetter
    {
        public string alphabet;
        public int[] letterCounter;

        public AlphabetSingleLetter(string alphabet)
        {
            this.alphabet = alphabet;
            letterCounter = new int[alphabet.Length];
        }
    }
}
