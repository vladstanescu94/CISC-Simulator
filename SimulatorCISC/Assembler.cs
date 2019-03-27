using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatorCISC
{
    class Assembler
    {
        Encodings encoding = new Encodings();

        private string fileName;
        private List<List<string>> asmInstructionLines = new List<List<string>>();
        private Dictionary<string, int> labelsDictionary = new Dictionary<string, int>();
        private List<string> machineCodeList = new List<string>();
        public short[] binaryMachineCodeArray;
        static public int StartAddress;

        public List<List<string>> AsmInstructionLines { get => asmInstructionLines; set => asmInstructionLines = value; }
        public List<string> MachineCodeList { get => machineCodeList; set => machineCodeList = value; }

    }
}
