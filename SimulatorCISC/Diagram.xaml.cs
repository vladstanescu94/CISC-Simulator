using System;
using System.Linq;
using System.Windows;

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

        private void BtnViewHex_Click(object sender, RoutedEventArgs e)
        {
            tbPC.Text = Convert.ToString(PC, 16).PadLeft(4, '0').ToUpper();
            tbIR.Text = Convert.ToString(IR, 16).PadLeft(4, '0').ToUpper();
            tbFlag.Text = Convert.ToString(FLAG, 16).PadLeft(4, '0').ToUpper();
            tbSP.Text = Convert.ToString(SP, 16).PadLeft(4, '0').ToUpper();
            if (tbRG.Text.Length != 0)
            {
                string[] aux = tbRG.Text.Split(' ');
                int a;
                if (aux[0].Length == 3)
                    a = Convert.ToInt32(tbRG.Text.Substring(1, 1));
                else
                    a = Convert.ToInt32(tbRG.Text.Substring(1, 2));
                tbRG.Text = aux[0] + " " + Convert.ToString(RG[a], 16).PadLeft(4, '0').ToUpper();
            }
            tbT.Text = Convert.ToString(T, 16).PadLeft(4, '0').ToUpper();
            tbIVR.Text = Convert.ToString(IVR, 16).PadLeft(4, '0').ToUpper();
            tbADR.Text = Convert.ToString(ADR, 16).PadLeft(4, '0').ToUpper();
            tbMDR.Text = Convert.ToString(MDR, 16).PadLeft(4, '0').ToUpper();

            tbR0.Text = Convert.ToString(RG[0], 16).PadLeft(4, '0').ToUpper();
            tbR1.Text = Convert.ToString(RG[1], 16).PadLeft(4, '0').ToUpper();
            tbR2.Text = Convert.ToString(RG[2], 16).PadLeft(4, '0').ToUpper();
            tbR3.Text = Convert.ToString(RG[3], 16).PadLeft(4, '0').ToUpper();
            tbR4.Text = Convert.ToString(RG[4], 16).PadLeft(4, '0').ToUpper();
            tbR5.Text = Convert.ToString(RG[5], 16).PadLeft(4, '0').ToUpper();
            tbR6.Text = Convert.ToString(RG[6], 16).PadLeft(4, '0').ToUpper();
            tbR7.Text = Convert.ToString(RG[7], 16).PadLeft(4, '0').ToUpper();
            tbR8.Text = Convert.ToString(RG[8], 16).PadLeft(4, '0').ToUpper();
            tbR9.Text = Convert.ToString(RG[9], 16).PadLeft(4, '0').ToUpper();
            tbR10.Text = Convert.ToString(RG[10], 16).PadLeft(4, '0').ToUpper();
            tbR11.Text = Convert.ToString(RG[11], 16).PadLeft(4, '0').ToUpper();
            tbR12.Text = Convert.ToString(RG[12], 16).PadLeft(4, '0').ToUpper();
            tbR13.Text = Convert.ToString(RG[13], 16).PadLeft(4, '0').ToUpper();
            tbR14.Text = Convert.ToString(RG[14], 16).PadLeft(4, '0').ToUpper();
            tbR15.Text = Convert.ToString(RG[15], 16).PadLeft(4, '0').ToUpper();



            tbMemoryDisplay.Text = "";
            string aux1 = "";
            for (int j = 0; j < 5000; j++)
            {
                aux1 += j + ": " + Convert.ToString(MEMORIE[j], 16).PadLeft(4, '0').ToUpper();
                aux1 += Environment.NewLine;

            }
            tbMemoryDisplay.Text = aux1;
            tbMPM.Text = "";
            aux1 = "";
            for (int j = 0; j < 173; j++)
            {
                aux1 += j + ": " + Convert.ToString(MPM[j], 16).PadLeft(10, '0').ToUpper();
                aux1 += Environment.NewLine;
            }
            tbMPM.Text = aux1;
        }
        private void BtnViewBinary_Click(object sender, RoutedEventArgs e)
        {
            tbPC.Text = Convert.ToString(PC, 2).PadLeft(16, '0');
            tbIR.Text = Convert.ToString(IR, 2).PadLeft(16, '0');
            tbFlag.Text = Convert.ToString(FLAG, 2).PadLeft(16, '0');
            tbSP.Text = Convert.ToString(SP, 2).PadLeft(16, '0');
            if (tbRG.Text.Length != 0)
            {
                string[] aux = tbRG.Text.Split(' ');
                int a;
                if (aux[0].Length == 3)
                    a = Convert.ToInt32(tbRG.Text.Substring(1, 1));
                else
                    a = Convert.ToInt32(tbRG.Text.Substring(1, 2));
                tbRG.Text = aux[0] + " " + Convert.ToString(RG[a], 2).PadLeft(16, '0');
            }
            tbT.Text = Convert.ToString(T, 2).PadLeft(16, '0');
            tbIVR.Text = Convert.ToString(IVR, 2).PadLeft(16, '0');
            tbADR.Text = Convert.ToString(ADR, 2).PadLeft(16, '0');
            tbMDR.Text = Convert.ToString(MDR, 2).PadLeft(16, '0');

            tbR0.Text = Convert.ToString(RG[0], 2).PadLeft(16, '0');
            tbR1.Text = Convert.ToString(RG[1], 2).PadLeft(16, '0');
            tbR2.Text = Convert.ToString(RG[2], 2).PadLeft(16, '0');
            tbR3.Text = Convert.ToString(RG[3], 2).PadLeft(16, '0');
            tbR4.Text = Convert.ToString(RG[4], 2).PadLeft(16, '0');
            tbR5.Text = Convert.ToString(RG[5], 2).PadLeft(16, '0');
            tbR6.Text = Convert.ToString(RG[6], 2).PadLeft(16, '0');
            tbR7.Text = Convert.ToString(RG[7], 2).PadLeft(16, '0');
            tbR8.Text = Convert.ToString(RG[8], 2).PadLeft(16, '0');
            tbR9.Text = Convert.ToString(RG[9], 2).PadLeft(16, '0');
            tbR10.Text = Convert.ToString(RG[10], 2).PadLeft(16, '0');
            tbR11.Text = Convert.ToString(RG[11], 2).PadLeft(16, '0');
            tbR12.Text = Convert.ToString(RG[12], 2).PadLeft(16, '0');
            tbR13.Text = Convert.ToString(RG[13], 2).PadLeft(16, '0');
            tbR14.Text = Convert.ToString(RG[14], 2).PadLeft(16, '0');
            tbR15.Text = Convert.ToString(RG[15], 2).PadLeft(16, '0');



            tbMemoryDisplay.Text = "";
            string aux1 = "";
            for (int j = 0; j < 5000; j++)
            {
                aux1 += j + ": " + Convert.ToString(MEMORIE[j], 2).PadLeft(16, '0');
                aux1 += Environment.NewLine;

            }
            tbMemoryDisplay.Text = aux1;
            tbMPM.Text = "";
            aux1 = "";
            for (int j = 0; j < 173; j++)
            {
                aux1 += j + ": " + Convert.ToString(MPM[j], 2).PadLeft(39, '0');
                aux1 += Environment.NewLine;
            }
            tbMPM.Text = aux1;
        }
    }

}
