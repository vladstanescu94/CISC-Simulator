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
            GetLabels();

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
                        } else if (element.Contains("("))  { // register AI
                            string[] items = element.Split('(', ')').Where(x => x != "").ToArray();
                            if(items.Count() == 1){
                                binaryRegAndMA += "10" + encoding.registerDictionary[items[0]];
                            } else {
                                foreach (var item in items){
                                    if (encoding.registerDictionary.ContainsKey(item)){
                                        binaryRegAndMA += "11" + encoding.registerDictionary[item];
                                    } else {
                                        binaryIndexList.Add(Convert.ToString(Convert.ToUInt16(item), 2).PadLeft(16, '0'));
                                    }
                                }
                            }
                        } else if (IsMAI(element)) { // adresare imediata
                            binaryRegAndMA += "00" + "0000";
                            binaryIndexList.Add(Convert.ToString(Convert.ToUInt16(element), 2).PadLeft(16, '0'));
                        } else if (element.Contains(":")) {
                            isLabel = true;
                            continue;
                        } else if (IsLabelInInstruction(element)) {
                            int offset = (line.Contains("JMP") || line.Contains("CALL")) ? labelsDictionary[element] + Masks.ADR_START_MEMORIE : labelsDictionary[element] - machineCodeList.Count - 1;
                            if (line.Contains("JMP") || line.Contains("CALL")) {
                                binaryRegAndMA += "00" + "0000";
                                binaryIndexList.Add(Convert.ToString((Int16)offset, 2).PadLeft(16, '0'));
                            }else {
                                if (offset < -255 || offset > 255) { throw new Exception("Offset 8 bits out of range!"); }
                                binaryOffset = Convert.ToString((byte)offset, 2).PadLeft(8, '0');
                            }
                        } else if (element == "END"){
                            StartAddress = (line.Count == 1) ? 0 : labelsDictionary[line.Last()];
                            isLabel = true;
                            continue;
                        } else {
                            Exception exc = new Exception("Parsed File Code is  wrong");
                            throw exc;
                        }
                    }
                    if (isLabel) {
                        continue;
                    } else if (binaryRegAndMA.Any()) {
                        binaryLine = binaryOpCode + binaryRegAndMA.Substring(6) + binaryRegAndMA.Substring(0, 6); // B1 B2
                    } else if (binaryOffset.Any()) {
                        binaryLine = binaryOpCode + binaryOffset; //B3
                    } else { // B4
                        binaryLine = binaryOpCode;
                    }

                    MachineCodeList.Add(binaryLine);
                    binaryIndexList.Reverse();
                    binaryIndexList.ForEach(x => machineCodeList.Add(x));
                }
                binaryMachineCodeArray = machineCodeList.ConvertAll<short>(x => Convert.ToInt16(x, 2)).ToArray();
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsLabelInInstruction(string element) {
            return labelsDictionary.ContainsKey(element);
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
