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
        public MainWindow()
        {
            InitializeComponent();
        }
     
        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            parseInput(tbCode.Text);
            
        }

        private void BtnDiagram_Click(object sender, RoutedEventArgs e)
        {
            Diagram diagramWindow = new Diagram();
            diagramWindow.Show();
        }

        public void parseInput(string inputString) {
            try {
                tbDisplay.Text = "";
                inputString = inputString.ToUpper();
                string[] asmCode = inputString.Split(',');
                foreach (string firstSplit in asmCode)
                {
                    string[] temp = firstSplit.Split(' ');
                    foreach (string token in temp)
                    {
                        tbDisplay.Text += token + '\n';
                    }
                }
            } catch(Exception e) {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = dlg.ShowDialog();
            if (result.HasValue && result.Value) {
                tbCode.Text = System.IO.File.ReadAllText(dlg.FileName).ToUpper();
            }
        }
    }
}
