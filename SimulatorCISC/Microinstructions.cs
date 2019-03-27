using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatorCISC
{
    class Microinstructions
    {

        public Dictionary<String, String> dbus = new Dictionary<string, string>();
        public Dictionary<String, String> cod_Ram = new Dictionary<string, string>();
        public Dictionary<String, String> destRbus = new Dictionary<string, string>();
        public Dictionary<String, String> memoryOperation = new Dictionary<string, string>();
        public Dictionary<String, String> nTF = new Dictionary<string, string>();
        public Dictionary<String, String> operatie_Alu = new Dictionary<string, string>();
        public Dictionary<String, String> other_op = new Dictionary<string, string>();
        public Dictionary<String, String> sbus = new Dictionary<string, string>();
        public Dictionary<String, String> index = new Dictionary<string, string>();
        public Dictionary<String, int> etichete = new Dictionary<string, int>();

        string[] microprogramDeEmulare = new string[173];
        public long[] microprogramEmulare = new long[173];
        string linieMicroprogram;

        public Microinstructions() {
            string pathDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\", "Encoding"));
            string[] lines = File.ReadAllLines(pathDirectory + "\\Cod_Ram_if.txt");

            foreach (string line in lines) {
                string[] temp = line.Split(';');
                cod_Ram.Add(temp[0], temp[1]);
            }

            lines = File.ReadAllLines(pathDirectory + "\\DBus.txt");

            foreach (string line in lines){
                string[] temp = line.Split(';');
                dbus.Add(temp[0], temp[1]);
            }

            lines = File.ReadAllLines(pathDirectory + "\\DestRbus.txt");

            foreach (string line in lines){
                string[] temp = line.Split(';');
                destRbus.Add(temp[0], temp[1]);
            }

            lines = File.ReadAllLines(pathDirectory + "\\MemoryOperation.txt");

            foreach (string line in lines){
                string[] temp = line.Split(';');
                memoryOperation.Add(temp[0], temp[1]);
            }

            lines = File.ReadAllLines(pathDirectory + "\\nT_F.txt");

            foreach (string line in lines){
                string[] temp = line.Split(';');
                nTF.Add(temp[0], temp[1]);
            }

            lines = File.ReadAllLines(pathDirectory + "\\Operatie_Alu.txt");

            foreach (string line in lines){
                string[] temp = line.Split(';');
                operatie_Alu.Add(temp[0], temp[1]);
            }

            lines = File.ReadAllLines(pathDirectory + "\\Other_Op.txt");

            foreach (string line in lines){
                string[] temp = line.Split(';');
                other_op.Add(temp[0], temp[1]);
            }

            lines = File.ReadAllLines(pathDirectory + "\\Sbus.txt");

            foreach (string line in lines){
                string[] temp = line.Split(';');
                sbus.Add(temp[0], temp[1]);
            }

            lines = File.ReadAllLines(pathDirectory + "\\Index.txt");

            foreach (string line in lines){
                string[] temp = line.Split(';');
                index.Add(temp[0], temp[1]);
            }

            int ct = 0;
            lines = File.ReadAllLines(pathDirectory + "\\Microprogram_De_Emulare.txt");

            foreach (string line in lines){
                if (line.Contains(':')){
                    string[] temp = line.Split(':');
                    etichete.Add(temp[0], ct);
                }
                ct++;
            }

            int ct2 = 0;
            foreach (string line in lines){

                if (line.Contains(':')){

                    string[] temp = line.Split(':');
                    string[] temp2 = temp[1].Split(';');
                    if (!temp2.Contains("STEP")){
                        string a = sbus[temp2[0]];
                        a = dbus[temp2[1]];
                        a = operatie_Alu[temp2[2]];
                        a = destRbus[temp2[3]];
                        a = other_op[temp2[4]];
                        a = memoryOperation[temp2[5]];
                        a = cod_Ram[temp2[6]];
                        linieMicroprogram = sbus[temp2[0]] + dbus[temp2[1]] + operatie_Alu[temp2[2]] + destRbus[temp2[3]] + other_op[temp2[4]] + memoryOperation[temp2[5]] + cod_Ram[temp2[6]] + nTF[temp2[7]] + index[temp2[8]] + Convert.ToString(etichete[temp2[9]], 2).PadLeft(8, '0');
                    } else {
                        linieMicroprogram = sbus[temp2[0]] + dbus[temp2[1]] + operatie_Alu[temp2[2]] + destRbus[temp2[3]] + other_op[temp2[4]] + memoryOperation[temp2[5]] + cod_Ram[temp2[6]] + "000000000000";
                    }

                } else {
                    string[] temp2 = line.Split(';');
                    if (!temp2.Contains("STEP"))
                        linieMicroprogram = sbus[temp2[0]] + dbus[temp2[1]] + operatie_Alu[temp2[2]] + destRbus[temp2[3]] + other_op[temp2[4]] + memoryOperation[temp2[5]] + cod_Ram[temp2[6]] + nTF[temp2[7]] + index[temp2[8]] + Convert.ToString(etichete[temp2[9]], 2).PadLeft(8, '0');
                    else
                        linieMicroprogram = sbus[temp2[0]] + dbus[temp2[1]] + operatie_Alu[temp2[2]] + destRbus[temp2[3]] + other_op[temp2[4]] + memoryOperation[temp2[5]] + cod_Ram[temp2[6]] + "000000000000";

                }
                microprogramDeEmulare[ct2++] = linieMicroprogram;

            }
            for (int i = 0; i < 173; i++){
                microprogramEmulare[i] = Convert.ToInt64(microprogramDeEmulare[i], 2);
            }

        }

    }
}
