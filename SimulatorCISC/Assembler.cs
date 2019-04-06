using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimulatorCISC
{
    class Assembler
    {
        Encodings encoding = new Encodings();

        public string fileName;
        private List<List<string>> asmInstructionLines = new List<List<string>>();
        private Dictionary<string, int> labelsDictionary = new Dictionary<string, int>();
        private List<string> machineCodeList = new List<string>();
        public short[] binaryMachineCodeArray;
        static public int StartAddress;

        public List<List<string>> AsmInstructionLines { get => asmInstructionLines; set => asmInstructionLines = value; }
        public List<string> MachineCodeList { get => machineCodeList; set => machineCodeList = value; }

        public bool ParseASMFile() {
            try {
                asmInstructionLines.Clear();
                string[] lines = File.ReadAllLines(fileName);

                if (lines.Any()) {
                    foreach(string line in lines){
                        string noCommentsLine = DeleteComments(line.ToUpper());
                        char[] delimiters = {' ', ',', '\t' };
                        string[] asmCode = noCommentsLine.Split(delimiters);

                        if (asmCode.All(y => string.IsNullOrEmpty(y))) {
                            continue;
                        }
                        asmInstructionLines.Add(asmCode.Where(x => !string.IsNullOrWhiteSpace(x)).ToList());
                    }
                    return true;
                } else{
                    return false;
                }
            } catch (Exception e){
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private string DeleteComments(string line){
            if (line.Contains(";")){
                string temp = line.Remove(line.IndexOf(';'));
                return temp;
            }
            return line;
        }

        private void GetLabels(){
            int labelAddr= 0;
            foreach (var line in asmInstructionLines) {
                foreach (var element in line) {
                    if (element.Contains(":")) {
                        string label = element.Remove(element.Length - 1);
                        labelsDictionary.Add(label, labelAddr);
                        labelAddr--;
                    } else if (element.Contains("(") && (element.First() != '(' || element.Last() != ')')) { //mod adresare indexat
                        labelAddr++;
                    } else if (IsMAI(element)){ // adresare imediata
                        labelAddr++;
                    } else if ((element == "JMP" || element == "CALL") && !line.Last().Contains("(")){ //jmp sau call la adresa direct
                        labelAddr++;
                    }
                }
                labelAddr++;
            }
        }

        private bool IsMAI(string element) {
            return int.TryParse(element, out int intResult);
        }

        public void GenerateMachineCode() {
            machineCodeList.Clear();

            string binaryLine;
            string binaryOpCode;
            string binaryRegAndMA;
            string binaryOffset;
            bool isLabel;
            List<string> binaryIndexList = new List<string>();

            try {
                foreach (var line in asmInstructionLines) {
                    binaryLine = "";
                    binaryOpCode = "";
                    binaryRegAndMA = "";
                    binaryOffset = "";
                    isLabel = false;
                    binaryIndexList.Clear();

                    foreach (string element in line) {
                        //check if instruction
                        if (encoding.instructionDictionary.ContainsKey(element)){
                            binaryOpCode = encoding.instructionDictionary[element];
                        } else if (encoding.registerDictionary.ContainsKey(element)){ // check if is register (adresare directa)
                            binaryRegAndMA += "01" + encoding.registerDictionary[element];
                        }
                    }
                }
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void WriteBinaryFile(string filePath) {
            using (var bw = new BinaryWriter(new FileStream(filePath, FileMode.Create))) {
                foreach (var item in binaryMachineCodeArray) {
                    bw.Write(item);
                }
            }
        }
    }
}
