using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimulatorCISC
{
    class Encodings
    {
        public Dictionary<string, string> instructionDictionary = new Dictionary<string, string>();
        public Dictionary<string, string> registerDictionary = new Dictionary<string, string>();

        public Encodings() {
            string instructionsPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\", @"Instructions\instructions.txt"));
            string registersPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\", @"Instructions\registers.txt"));
            ReadInstructionsFile(instructionsPath);
            ReadRegistersFile(registersPath);
        }

        private void ReadInstructionsFile(string path) {
            try{
                foreach (string line in File.ReadLines(path))
                {
                    string[] tokens = line.Split(' ');
                    instructionDictionary.Add(tokens[0], tokens[1]);
                }
            } catch (Exception e){
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReadRegistersFile(string path)
        {
            try{
                foreach (string line in File.ReadLines(path))
                {
                    string[] tokens = line.Split(' ');
                    registerDictionary.Add(tokens[0], tokens[1]);
                }
            } catch(Exception e){
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
