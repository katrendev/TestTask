using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask.CharacterMachines
{
    internal class DoubleCharacterMachine : ICharacterMachine
    {
        private char _previous;

        private string _currentString;

        public string GetCurrentString() => _currentString;

        public bool CheckChar(char c)
        {
            if (!c.IsCyrillic() || !Matches(c))
            {
                _previous = c;
                return false;
            }

            _currentString = c.ToString().ToLower();
            return true;

        }

        public void Reset()
        {
            _previous = default(char);
            _currentString = null;
        }

        private bool Matches(char c) => char.ToLower(c) == char.ToLower(_previous);
    }
}
