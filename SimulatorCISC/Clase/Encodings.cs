using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace SimulatorCISC
{
    class Encodings
    {
        public Dictionary<string, string> insDictionary = new Dictionary<string, string>();
        public Dictionary<string, string> regDictionary = new Dictionary<string, string>();

        public Encodings() {
            string instructionsPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\", @"Instructions\instructions.txt"));
            string registersPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\", @"Instructions\registers.txt"));
            ParseInstructionsFile(instructionsPath);
            ParseRegistersFile(registersPath);
        }

        private void ParseInstructionsFile(string path) {
            try{
                foreach (string line in File.ReadLines(path))
                {
                    string[] tokens = line.Split(' ');
                    insDictionary.Add(tokens[0], tokens[1]);
                }
            } catch (Exception e){
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ParseRegistersFile(string path)
        {
            try{
                foreach (string line in File.ReadLines(path))
                {
                    string[] tokens = line.Split(' ');
                    regDictionary.Add(tokens[0], tokens[1]);
                }
            } catch(Exception e){
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
