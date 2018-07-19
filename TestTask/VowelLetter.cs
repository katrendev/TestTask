using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask
{
    public class VowelLetter : AlphabetLetter
    {
        public VowelLetter(string alphabet) : base(alphabet)
        {
            alphabet = "аеёиоуыэюя";
        }
    }
}
