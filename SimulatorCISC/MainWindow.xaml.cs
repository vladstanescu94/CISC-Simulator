using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        }
     
        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            parseInput();
            
            
        }

        private void BtnDiagram_Click(object sender, RoutedEventArgs e)
        {
            Diagram diagramWindow = new Diagram();
            diagramWindow.Show();
        }

        public void parseInput() {
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
            foreach (string line in assembler.MachineCodeList){
                tbBinary.Text += line + "\r\n";
            }
        }

        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            try{
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Filter = "Assembler Source File input(*.asm)|*.asm";
                Nullable<bool> result = dlg.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    assembler.fileName = dlg.FileName;
                    tbCode.Text = System.IO.File.ReadAllText(dlg.FileName).ToUpper();
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
