using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask.CharacterMachines
{
    internal class SimpleCharacterMachine : ICharacterMachine
    {
        private string _currentString;

        public string GetCurrentString() => _currentString;

        public bool CheckChar(char c)
        {
            if (!c.IsCyrillic())
            {
                return false;
            }

            _currentString = c.ToString();
            return true;
        }

        public void Reset()
        {
            _currentString = null;
        }
    }
}
