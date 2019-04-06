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
using System.Windows.Shapes;

namespace SimulatorCISC
{
    /// <summary>
    /// Interaction logic for Diagram.xaml
    /// </summary>
    public partial class Diagram : Window
    {
        short[] codBinar;
        private static Int16[] MEMORIE = new Int16[65536];
        private Int16 IR;
        private UInt16 PC;
        private UInt16 FLAG;
        private Int16 T;
        private UInt16 SP;
        private UInt16 ADR;
        private Int16 MDR;
        private UInt16 IVR;
        private Int16[] RG = new Int16[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int variabila = 0;
        private static long[] MPM = new long[173];
        private byte MAR;
        private Int16 MIR;
        private Int16 SBUS;
        private Int16 DBUS;
        private Int16 RBUS;
        private byte g;
        private byte state;
        private UInt16 C, Z, S, V;
        private byte INTR;
        private byte BVI, BPO;
        private UInt16 ultimaAdresaMemorie;
        private bool instructiuneFinalizata = false;
        Microinstructions dictionarMicroprogram;

        public Diagram(short[] code)
        {
            InitializeComponent();
            try {
                codBinar = new short[code.Length];
                code.CopyTo(codBinar, 0);
            } catch (Exception e){
                MessageBox.Show("Please execute code first! " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            dictionarMicroprogram = new Microinstructions();
            dictionarMicroprogram.microprogramEmulare.CopyTo(MPM, 0);
            Initializare();

            //set RG
            tbR0.Text = RG[0].ToString();
            tbR1.Text = RG[1].ToString();
            tbR2.Text = RG[2].ToString();
            tbR3.Text = RG[3].ToString();
            tbR4.Text = RG[4].ToString();
            tbR5.Text = RG[5].ToString();
            tbR6.Text = RG[6].ToString();
            tbR7.Text = RG[7].ToString();
            tbR8.Text = RG[8].ToString();
            tbR9.Text = RG[9].ToString();
            tbR10.Text = RG[10].ToString();
            tbR11.Text = RG[11].ToString();
            tbR12.Text = RG[12].ToString();
            tbR13.Text = RG[13].ToString();
            tbR14.Text = RG[14].ToString();
            tbR15.Text = RG[15].ToString();

            tbPC.Text = PC.ToString();
            tbSP.Text = SP.ToString();
            tbMemoryDisplay.Text = "";
            string temp = "";
            for (int i = 0; i< 5000; i ++) {
                temp += i + ": " + MEMORIE[i].ToString();
                temp += Environment.NewLine;
            }
            tbMemoryDisplay.Text = temp;
            tbMPM.Text = "";
            temp = "";

            for (int i = 0; i < 173; i++)
            {
                temp += i + ": " + MPM[i].ToString().PadLeft(16,'0');
                temp += Environment.NewLine;
            }
            tbMPM.Text = temp;

        }

        private void BtnExecuteInstruction_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Initializare() {
            PC = (ushort)(Masks.ADR_START_MEMORIE + Assembler.StartAddress);
            try {
                codBinar.CopyTo(MEMORIE, PC);
                ultimaAdresaMemorie = (ushort)(PC + codBinar.Count());
            } catch (Exception e){
                MessageBox.Show("Machine code not found! " + e.Message , "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            //IVT
            MEMORIE[0] = 5;
            MEMORIE[5] = Convert.ToInt16("1110000000001001", 2); //SCC
            MEMORIE[6] = Convert.ToInt16("1110000000001010", 2); //NOP
            MEMORIE[7] = Convert.ToInt16("1110000000001100", 2); //Reti

            SP = 3500;
            MAR = 0;
            IVR = 0;
            INTR = 0;
            BVI = 1; //validare  intrerupere
            BPO = 1; //halt program
            C = V = S = Z = 0;
            state = 0;

            //for (int i = 0; i < 1000; i ++) {
            //    tbMemoryDisplay.Text += i + ": " + MEMORIE[i].ToString();
            //    tbMemoryDisplay.Text += Environment.NewLine; 
            //}

        }
    }
}
