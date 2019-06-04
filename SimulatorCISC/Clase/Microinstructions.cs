using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimulatorCISC
{
    class Microinstructions
    {
        public Dictionary<String, long> dbus = new Dictionary<string, long>();
        public Dictionary<String, long> cod_Ram = new Dictionary<string, long>();
        public Dictionary<String, long> destRbus = new Dictionary<string, long>();
        public Dictionary<String, long> memoryOperation = new Dictionary<string, long>();
        public Dictionary<String, long> nTF = new Dictionary<string, long>();
        public Dictionary<String, long> operatie_Alu = new Dictionary<string, long>();
        public Dictionary<String, long> other_op = new Dictionary<string, long>();
        public Dictionary<String, long> sbus = new Dictionary<string, long>();
        public Dictionary<String, long> index = new Dictionary<string, long>();
        public Dictionary<String, int> etichete = new Dictionary<string, int>();

        public long[] microprogramEmulare = new long[173];
        long linieMicroprogram;

        public Microinstructions() {
            string pathDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\", "Encoding"));
            string[] lines = File.ReadAllLines(pathDirectory + "\\Cod_Ram_if.txt");

            foreach (string line in lines) {
                string[] substring = line.Split(';');
                cod_Ram.Add(substring[0], Convert.ToInt64(substring[1]));
            }

            lines = File.ReadAllLines(pathDirectory + "\\DBus.txt");

            foreach (string line in lines){
                string[] substring = line.Split(';');
                dbus.Add(substring[0], Convert.ToInt64(substring[1]));
            }

            lines = File.ReadAllLines(pathDirectory + "\\DestRbus.txt");

            foreach (string line in lines){
                string[] substring = line.Split(';');
                destRbus.Add(substring[0], Convert.ToInt64(substring[1]));
            }

            lines = File.ReadAllLines(pathDirectory + "\\MemoryOperation.txt");

            foreach (string line in lines){
                string[] substring = line.Split(';');
                memoryOperation.Add(substring[0], Convert.ToInt64(substring[1]));
            }

            lines = File.ReadAllLines(pathDirectory + "\\nT_F.txt");

            foreach (string line in lines){
                string[] substring = line.Split(';');
                nTF.Add(substring[0], Convert.ToInt64(substring[1]));
            }

            lines = File.ReadAllLines(pathDirectory + "\\Operatie_Alu.txt");

            foreach (string line in lines){
                string[] substring = line.Split(';');
                operatie_Alu.Add(substring[0], Convert.ToInt64(substring[1]));
            }

            lines = File.ReadAllLines(pathDirectory + "\\Other_Op.txt");

            foreach (string line in lines){
                string[] substring = line.Split(';');
                other_op.Add(substring[0], Convert.ToInt64(substring[1]));
            }

            lines = File.ReadAllLines(pathDirectory + "\\Sbus.txt");

            foreach (string line in lines){
                string[] substring = line.Split(';');
                sbus.Add(substring[0], Convert.ToInt64(substring[1]));
            }

            lines = File.ReadAllLines(pathDirectory + "\\Index.txt");

            foreach (string line in lines){
                string[] substring = line.Split(';');
                index.Add(substring[0], Convert.ToInt32(substring[1]));
            }

            int ct = 0;
            lines = File.ReadAllLines(pathDirectory + "\\Microprogram_De_Emulare.txt");

            foreach (string line in lines){
                if (line.Contains(':')){
                    string[] substring = line.Split(':');
                    etichete.Add(substring[0], ct);
                }
                ct++;
            }

            int ct2 = 0;
            foreach (string line in lines){

                if (line.Contains(':'))
                {

                    string[] microInstruction = line.Split(':');          // separate the label from microintruction
                    string[] fields = microInstruction[1].Split(';');       // microinstruction itself splitted in fields

                    if (!fields.Contains("STEP"))
                    {

                        linieMicroprogram = sbus[fields[0]] << (int)Masks.Position.SBUS | dbus[fields[1]] << (int)Masks.Position.DBUS |
                                                  operatie_Alu[fields[2]] << (int)Masks.Position.ALU | destRbus[fields[3]] << (int)Masks.Position.RBUS |
                                                  other_op[fields[4]] << (int)Masks.Position.ALTEOP | memoryOperation[fields[5]] << (int)Masks.Position.OPMEMORIE |
                                                  cod_Ram[fields[6]] << (int)Masks.Position.F | nTF[fields[7]] << (int)Masks.Position.NTF |
                                                  index[fields[8]] << (int)Masks.Position.INDEX | etichete[fields[9]];
                    }
                    else
                    {
                        linieMicroprogram = sbus[fields[0]] << (int)Masks.Position.SBUS | dbus[fields[1]] << (int)Masks.Position.DBUS |
                                                  operatie_Alu[fields[2]] << (int)Masks.Position.ALU | destRbus[fields[3]] << (int)Masks.Position.RBUS |
                                                  other_op[fields[4]] << (int)Masks.Position.ALTEOP | memoryOperation[fields[5]] << (int)Masks.Position.OPMEMORIE |
                                                  cod_Ram[fields[6]] << (int)Masks.Position.F;
                    }

                }
                else
                {

                    string[] fields = line.Split(';');

                    if (!fields.Contains("STEP"))
                    {

                        linieMicroprogram = sbus[fields[0]] << (int)Masks.Position.SBUS | dbus[fields[1]] << (int)Masks.Position.DBUS |
                                                  operatie_Alu[fields[2]] << (int)Masks.Position.ALU | destRbus[fields[3]] << (int)Masks.Position.RBUS |
                                                  other_op[fields[4]] << (int)Masks.Position.ALTEOP | memoryOperation[fields[5]] << (int)Masks.Position.OPMEMORIE |
                                                  cod_Ram[fields[6]] << (int)Masks.Position.F | nTF[fields[7]] << (int)Masks.Position.NTF |
                                                  index[fields[8]] << (int)Masks.Position.INDEX | etichete[fields[9]];
                    }
                    else
                    {
                        linieMicroprogram = sbus[fields[0]] << (int)Masks.Position.SBUS | dbus[fields[1]] << (int)Masks.Position.DBUS |
                                                  operatie_Alu[fields[2]] << (int)Masks.Position.ALU | destRbus[fields[3]] << (int)Masks.Position.RBUS |
                                                  other_op[fields[4]] << (int)Masks.Position.ALTEOP | memoryOperation[fields[5]] << (int)Masks.Position.OPMEMORIE |
                                                  cod_Ram[fields[6]] << (int)Masks.Position.F;
                    }

                }
                microprogramEmulare[ct2++] = linieMicroprogram;

            }

        }

    }
}
