using System;
using System.Windows;
using System.IO;
using System.Linq;

namespace SimulatorCISC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Assembler assembler = new Assembler();
        public MainWindow()
        {
            InitializeComponent();
            btnDiagram.IsEnabled = false;
            btnExecute.IsEnabled = false;
        }
     
        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            executeASMFile();
            btnDiagram.IsEnabled = true;
        }

        private void BtnDiagram_Click(object sender, RoutedEventArgs e)
        {
            Diagram diagramWindow = new Diagram(assembler.binaryMachineCodeArray);
            diagramWindow.Show();
        }

        public void executeASMFile() {
            //TODO: Change parsing
            tbDisplay.Text = "";
            if (assembler.ParseASMFile()) {
                foreach (var line in assembler.AsmInstructionLines) {
                    foreach (var temp in line) {
                        tbDisplay.Text += temp + "\r\n";
                    }
                }
                generateBinary();
            } else{
                MessageBox.Show("Parsing Failed", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void generateBinary() {
            tbBinary.Text = "";
            assembler.GenerateMachineCode();
            writeBinaryFile();
            foreach (string line in assembler.MachineCodeList){
                tbBinary.Text += line + "\r\n";
            }
        }

        private void writeBinaryFile(){
            string binaryFilePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\", @"Output\binaryOutput.bin"));
            try
            {
                assembler.WriteBinaryFile(binaryFilePath);
                MessageBox.Show("BIN file generated!", "Message", MessageBoxButton.OK);
            }
            catch (Exception e) {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            try{
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Assembler Source File input(*.asm)|*.asm"
                };
                bool? result = dlg.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    assembler.fileName = dlg.FileName;
                    tbCode.Text = File.ReadAllText(dlg.FileName).ToUpper();
                    btnExecute.IsEnabled = true;
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
