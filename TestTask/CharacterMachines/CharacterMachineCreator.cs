using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask.CharacterMachines
{
    internal static class CharacterMachineCreator
    {
        public static ICharacterMachine GetSimpleCharacterMachine() => new SimpleCharacterMachine();

        public static ICharacterMachine GetDoubleCharacterMachine() => new DoubleCharacterMachine();
    }
}
