using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
        private Int64 MIR;
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
        private List<TextBox> registersTBList = new List<TextBox>();

        Microinstructions dictionarMicroprogram;

        

        public Diagram(short[] code)
        {
            
            InitializeComponent();
            registersTBList.Add(tbR0);
            registersTBList.Add(tbR1);
            registersTBList.Add(tbR2);
            registersTBList.Add(tbR3);
            registersTBList.Add(tbR4);
            registersTBList.Add(tbR5);
            registersTBList.Add(tbR6);
            registersTBList.Add(tbR7);
            registersTBList.Add(tbR8);
            registersTBList.Add(tbR9);
            registersTBList.Add(tbR10);
            registersTBList.Add(tbR11);
            registersTBList.Add(tbR12);
            registersTBList.Add(tbR13);
            registersTBList.Add(tbR14);
            registersTBList.Add(tbR15);

            try {
                codBinar = new short[code.Length];
                code.CopyTo(codBinar, 0);
            } catch (Exception e){
                MessageBox.Show("Please execute code first! " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            dictionarMicroprogram = new Microinstructions();
            dictionarMicroprogram.microprogramEmulare.CopyTo(MPM, 0);
            Initializare();

            for (int i = 0; i < registersTBList.Count; i++){
                registersTBList[i].Text = RG[i].ToString();
            }

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
            if (PC == ultimaAdresaMemorie && instructiuneFinalizata == true) {
                MessageBox.Show("Simulation Done!", "Message", MessageBoxButton.OK);
                return;
            }
            instructiuneFinalizata = false;
            Seq();
        }

        private void Seq() {
            if (BPO == 0) {
                return;
            }
            switch (state) {
                case 0:
                    MIR = MPM[MAR];
                    ResetDisplay(MIR);
                    state = 1;
                    break;
                case 1:
                    g = GetGeneralInstruction(MIR);
                    if (g == 0) { MAR++; }
                    else { MAR = (byte)(GetMicroAddress(MIR) + GetIndex(MIR)); }
                    SBUS = GetSbus(MIR);
                    DBUS = GetDbus(MIR);
                    RBUS = ExecuteALU(MIR, SBUS, DBUS);
                    OutputRbus(MIR, RBUS);
                    OtherOperations(MIR);
                    MemoryOperations(MIR);
                    state = 0;
                    break;

                default:
                    throw new Exception("Invalid state number");
            }
        }

        private void MemoryOperations(long mIR)
        {
            lblMEMORY_OPS_Content.Background = new SolidColorBrush(Colors.Magenta);
            byte memory_codification = (byte)((mIR & Masks.MASCA_OPMEMORIE) >> Masks.POZITIE_OPMEMORIE);
            switch (memory_codification)
            {
                case 0://none
                    break;
                case 1: //IFCH
                    IR = (Int16)MEMORIE[ADR];
                    tbIR.Text = IR.ToString();
                    tbIR.Background = new SolidColorBrush(Colors.Magenta);
                    pMEMORY_IRMUX.Stroke = new SolidColorBrush(Colors.Magenta);
                    pIR_MUX.Stroke = new SolidColorBrush(Colors.Magenta);
                    pADR_MEMORY.Stroke = new SolidColorBrush(Colors.Magenta);
                    //forma
                    break;
                case 2://Read
                    MDR = MEMORIE[ADR];
                    pMEMORY_IRMUX.Stroke = new SolidColorBrush(Colors.Magenta);
                    pIR_MUX.Stroke = new SolidColorBrush(Colors.Magenta);
                    pADR_MEMORY.Stroke = new SolidColorBrush(Colors.Magenta);
                    pMUX_MDR.Stroke = new SolidColorBrush(Colors.Magenta);
                    tbMDR.Text = MDR.ToString();
                    tbMDR.Background = new SolidColorBrush(Colors.Magenta);
                    //forma
                    break;
                case 3://write
                    MEMORIE[ADR] = MDR;
                    pADR_MEMORY.Stroke = new SolidColorBrush(Colors.Magenta);
                    pMUX_MDR.Stroke = new SolidColorBrush(Colors.Magenta);
                    tbMemoryDisplay.Text = "";
                    string aux = "";
                    for (int i = 0; i < 5000; i++)
                    {
                        aux += i + ": " + MEMORIE[i].ToString();
                        aux += Environment.NewLine;

                    }
                    tbMemoryDisplay.Text = aux;
                    break;
                default:
                    throw new Exception("Wrong memory_codification in switch!");
            }
        }

        private void OtherOperations(long mIR)
        {
            lblOTHER_OP_Content.Background = new SolidColorBrush(Colors.Magenta);
            byte otherOps_codification = (byte)((mIR & Masks.MASCA_ALTEOP) >> Masks.POZITIE_ALTEOP);
            int flags;

            switch (otherOps_codification)
            {

                case 0://NONE
                    break;
                case 1://(cin,pdcond)   /cvzs
                    RBUS += 1;
                    if (RBUS == 0) { Z = 1; } else { Z = 0; }
                    if (RBUS < 0) { S = 1; } else { S = 0; }
                    if (((RBUS & 0x00010000) >> 16 == 1) && (RBUS & 0x0000FFFF) < 0x0000FFFF) { C = 1; } else { C = 0; }
                    if (((RBUS & 0xFFFF0000) >> 16 > 0) && (RBUS & 0x0000FFFF) == 0x0000FFFF) { V = 1; } else { V = 0; }
                    flags = (C << 3) | (V << 2) | (Z << 1) | (S << 0);
                    FLAG = (ushort)((FLAG & 0) | (ushort)flags);
                    pALU_MUX2.Stroke = new SolidColorBrush(Colors.Magenta);
                    //pRBUS_MUX2.Stroke = new SolidColorBrush(Colors.Magenta);
                    pMUX2_FLAG.Stroke = new SolidColorBrush(Colors.Magenta);
                    tbFlag.Text = FLAG.ToString();
                    tbFlag.Background = new SolidColorBrush(Colors.Magenta);
                    break;
                case 2://PdCOND
                    flags = (C << 3) | (V << 2) | (Z << 1) | (S << 0);
                    FLAG = (ushort)((FLAG & 0) | (ushort)flags);
                    pALU_MUX2.Stroke = new SolidColorBrush(Colors.Magenta);
                    //pRBUS_MUX2.Stroke = new SolidColorBrush(Colors.Magenta);
                    pMUX2_FLAG.Stroke = new SolidColorBrush(Colors.Magenta);
                    tbFlag.Text = FLAG.ToString();
                    tbFlag.Background = new SolidColorBrush(Colors.Magenta);
                    //forma
                    break;
                case 3://cin
                    RBUS += 1;
                    if (RBUS == 0) { Z = 1; } else { Z = 0; }
                    if (RBUS < 0) { S = 1; } else { S = 0; }
                    if (((RBUS & 0x00010000) >> 16 == 1) && (RBUS & 0x0000FFFF) < 0x0000FFFF) { C = 1; } else { C = 0; }
                    if (((RBUS & 0xFFFF0000) >> 16 > 0) && (RBUS & 0x0000FFFF) == 0x0000FFFF) { V = 1; } else { V = 0; }
                    flags = (C << 3) | (V << 2) | (Z << 1) | (S << 0);
                    FLAG = (ushort)((FLAG & 0) | (ushort)flags);
                    pALU_MUX2.Stroke = new SolidColorBrush(Colors.Magenta);
                    //pRBUS_MUX2.Stroke = new SolidColorBrush(Colors.Magenta);
                    pMUX2_FLAG.Stroke = new SolidColorBrush(Colors.Magenta);
                    tbFlag.Text = FLAG.ToString();
                    tbFlag.Background = new SolidColorBrush(Colors.Magenta);
                    break;
                case 4://+1SP
                    SP += 1;
                    tbSP.Text = SP.ToString();
                    tbSP.Background = new SolidColorBrush(Colors.Magenta);
                    break;
                case 5://-1SP
                    SP -= 1;
                    tbSP.Text = SP.ToString();
                    tbSP.Background = new SolidColorBrush(Colors.Magenta);
                    break;
                case 6://+1PC
                    PC += 1;
                    tbPC.Text = PC.ToString();
                    tbPC.Background = new SolidColorBrush(Colors.Magenta);
                    break;
                case 7://A(1)BVI
                    BVI = 1;
                    break;
                case 8://A(0)BVI  
                    BVI = 0;
                    break;
                case 9://A(0)BPO
                    BPO = 0;
                    break;
                case 10://A(1)C
                    C = 1;
                    FLAG = (ushort)(FLAG | (ushort)C << 3);
                    tbFlag.Text = FLAG.ToString();
                    tbFlag.Background = new SolidColorBrush(Colors.Magenta);
                    break;
                case 11://A(1)V
                    V = 1;
                    FLAG = (ushort)(FLAG | (ushort)V << 2);
                    tbFlag.Text = FLAG.ToString();
                    tbFlag.Background = new SolidColorBrush(Colors.Magenta);
                    break;
                case 12://A(1)Z
                    Z = 1;
                    FLAG = (ushort)(FLAG | (ushort)Z << 1);
                    tbFlag.Text = FLAG.ToString();
                    tbFlag.Background = new SolidColorBrush(Colors.Magenta);
                    break;
                case 13://A(1)S
                    S = 1;
                    FLAG = (ushort)(FLAG | (ushort)S);
                    tbFlag.Text = FLAG.ToString();
                    tbFlag.Background = new SolidColorBrush(Colors.Magenta);
                    break;
                case 14://A(1)CVZS
                    C = 1; V = 1; Z = 1; S = 1;
                    FLAG = (ushort)(FLAG | 0x000F);
                    tbFlag.Text = FLAG.ToString();
                    tbFlag.Background = new SolidColorBrush(Colors.Magenta);
                    break;
                case 15://A(0)C
                    C = 0;
                    FLAG = (ushort)(FLAG & 0xFFF7);
                    tbFlag.Text = FLAG.ToString();
                    tbFlag.Background = new SolidColorBrush(Colors.Magenta);
                    break;
                case 16://A(0)V
                    V = 0;
                    FLAG = (ushort)(FLAG & 0xFFF8);
                    tbFlag.Text = FLAG.ToString();
                    tbFlag.Background = new SolidColorBrush(Colors.Magenta);
                    break;
                case 17://A(0)Z
                    Z = 0;
                    FLAG = (ushort)(FLAG & 0xFFFD);
                    tbFlag.Text = FLAG.ToString();
                    tbFlag.Background = new SolidColorBrush(Colors.Magenta);
                    break;
                case 18://A(0)S
                    S = 0;
                    FLAG = (ushort)(FLAG & 0xFFFE);
                    tbFlag.Text = FLAG.ToString();
                    tbFlag.Background = new SolidColorBrush(Colors.Magenta);
                    break;
                case 19://A(0)CVZS
                    C = 0; V = 0; Z = 0; S = 0;
                    FLAG = (ushort)(FLAG & 0xFFF0);
                    tbFlag.Text = FLAG.ToString();
                    tbFlag.Background = new SolidColorBrush(Colors.Magenta);
                    break;
            }
        }

        private void OutputRbus(long mIR, short rBUS)
        {
            lblRBUS_Content.Background = new SolidColorBrush(Colors.Magenta);
            byte rbus_codification = (byte)((mIR & Masks.MASCA_RBUS) >> Masks.POZITIE_RBUS);
            int indexRG;
            pALU_RBUS.Stroke = new SolidColorBrush(Colors.Magenta);
            pRBUS.Stroke = new SolidColorBrush(Colors.Magenta);
            switch (rbus_codification)
            {
                case 0:
                    pALU_RBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pRBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    break;

                case 1://PmIR
                    IR = (short)rBUS;
                    tbIR.Text = IR.ToString();
                    tbIR.Background = new SolidColorBrush(Colors.Magenta);
                    break;
                case 2://PmT
                    T = rBUS;
                    pRBUS_T.Stroke = new SolidColorBrush(Colors.Magenta);
                    tbT.Text = T.ToString();
                    tbT.Background = new SolidColorBrush(Colors.Magenta);
                    break;
                case 3://PmSp
                    SP = (ushort)rBUS;
                    pRBUS_SP.Stroke = new SolidColorBrush(Colors.Magenta);
                    tbSP.Text = SP.ToString();
                    tbSP.Background = new SolidColorBrush(Colors.Magenta);
                    break;
                case 4: //PmMDR
                    MDR = rBUS;
                    pRBUS_MUX.Stroke = new SolidColorBrush(Colors.Magenta);
                    pMUX_MDR.Stroke = new SolidColorBrush(Colors.Magenta);
                    tbMDR.Text = MDR.ToString();
                    tbMDR.Background = new SolidColorBrush(Colors.Magenta);
                    break;
                case 5://PmRG
                    indexRG = IR & 0x000F;
                    RG[indexRG] = rBUS;
                    pRBUS_RG.Stroke = new SolidColorBrush(Colors.Magenta);
                    tbRG.Text = "R" + indexRG + ": " + RG[indexRG].ToString();
                    tbRG.Background = new SolidColorBrush(Colors.Magenta);
                    registersTBList[indexRG].Background = new SolidColorBrush(Colors.Magenta);
                    registersTBList[indexRG].Text = RG[indexRG].ToString();
                    break;
                case 6://PmPC
                    PC = (ushort)rBUS;
                    pRBUS_PC.Stroke = new SolidColorBrush(Colors.Magenta);
                    tbPC.Text = PC.ToString();
                    tbPC.Background = new SolidColorBrush(Colors.Magenta);
                    break;
                case 7://PmADR
                    ADR = (ushort)rBUS;
                    pRBUS_ADR.Stroke = new SolidColorBrush(Colors.Magenta);
                    tbADR.Text = ADR.ToString();
                    tbADR.Background = new SolidColorBrush(Colors.Magenta);
                    break;
                case 8://PmFLAG
                    FLAG = (ushort)rBUS;
                    C = (ushort)((FLAG & 0x0008) >> 3);
                    V = (ushort)((FLAG & 0x0004) >> 2);
                    Z = (ushort)((FLAG & 0x0002) >> 1);
                    S = (ushort)((FLAG & 0x0001));
                    pRBUS_MUX2.Stroke = new SolidColorBrush(Colors.Magenta);
                    pMUX2_FLAG.Stroke = new SolidColorBrush(Colors.Magenta);
                    tbFlag.Text = FLAG.ToString();
                    tbFlag.Background = new SolidColorBrush(Colors.Magenta);
                    break;
                default:
                    throw new Exception("Wrong rbus codification!");
            }
        }

        private short ExecuteALU(long mIR, short sBUS, short dBUS)
        {
            lblALU_Content.Background = new SolidColorBrush(Colors.Magenta);
            int flags;
            byte alu_codification = (byte)((mIR & Masks.MASCA_ALU) >> Masks.POZITIE_ALU);
            int rez;
            int copy_C;

            switch (alu_codification)
            {
                case 0:
                    pRBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    return 0;
                case 1://sbus
                    return sBUS;
                case 2://nsbus
                    return (short)(~sBUS);
                case 3://dbus
                    return dBUS;
                case 4://ndbus
                    return (short)(~dBUS);
                case 5://sum
                    rez = sBUS + dBUS;
                    if (rez == 0) { Z = 1; } else { Z = 0; }
                    if (rez < 0) { S = 1; } else { S = 0; }
                    if (((rez & 0x00010000) >> 16 == 1) && (rez & 0x0000FFFF) < 0x0000FFFF) { C = 1; } else { C = 0; }
                    if (((rez & 0xFFFF0000) >> 16 > 0) && (rez & 0x0000FFFF) == 0x0000FFFF) { V = 1; } else { V = 0; }
                    flags = (C << 3) | (V << 2) | (Z << 1) | (S << 0);
                    FLAG = (ushort)((FLAG & 0) | (ushort)flags);
                    return (short)rez;
                case 6://and
                    rez = sBUS & dBUS;
                    if (rez == 0) { Z = 1; } else { Z = 0; }
                    if (rez < 0) { S = 1; } else { S = 0; }
                    flags = (C << 3) | (V << 2) | (Z << 1) | (S << 0);
                    FLAG = (ushort)((FLAG & 0) | (ushort)flags);

                    return (short)rez;
                case 7://or
                    rez = sBUS | dBUS;
                    if (rez == 0) { Z = 1; } else { Z = 0; }
                    if (rez < 0) { S = 1; } else { S = 0; }
                    flags = (C << 3) | (V << 2) | (Z << 1) | (S << 0);
                    FLAG = (ushort)((FLAG & 0) | (ushort)flags);
                    
                    return (short)rez;
                case 8://xor
                    rez = sBUS ^ dBUS;
                    if (rez == 0) { Z = 1; } else { Z = 0; }
                    if (rez < 0) { S = 1; } else { S = 0; }
                    flags = (C << 3) | (V << 2) | (Z << 1) | (S << 0);
                    FLAG = (ushort)((FLAG & 0) | (ushort)flags);
              
                    return (short)rez;
                case 9://asl
                    C = (ushort)((dBUS * 0x8000) >> 15);
                    flags = (C << 3) | (V << 2) | (Z << 1) | (S << 0);
                    FLAG = (ushort)((FLAG & 0) | (ushort)flags);
                    return (short)(dBUS << 1);
                case 10://asr
                    C = (ushort)(dBUS * 0x0001);
                    flags = (C << 3) | (V << 2) | (Z << 1) | (S << 0);
                    FLAG = (ushort)((FLAG & 0) | (ushort)flags);
                    rez = dBUS & 0x8000; //msb
                    return (short)((dBUS >> 1) | rez);
                case 11://lsr
                    C = (ushort)(dBUS & 0x0001);
                    flags = (C << 3) | (V << 2) | (Z << 1) | (S << 0);
                    FLAG = (ushort)((FLAG & 0) | (ushort)flags);
                    return (short)(dBUS >> 1);
                case 12://rol
                    rez = (dBUS & 0x8000) >> 15;
                    C = (ushort)rez;
                    flags = (C << 3) | (V << 2) | (Z << 1) | (S << 0);
                    FLAG = (ushort)((FLAG & 0) | (ushort)flags);
                    return (short)((dBUS << 1) | rez);
                case 13://ror
                    rez = dBUS & 0x0001;
                    C = (ushort)rez;
                    flags = (C << 3) | (V << 2) | (Z << 1) | (S << 0);
                    FLAG = (ushort)((FLAG & 0) | (ushort)flags);
                    rez = rez << 15;
                    return (short)((dBUS >> 1) | rez);
                case 14: //rlc
                    rez = (dBUS & 0x8000) >> 15;
                    copy_C = C;
                    C = (ushort)rez;
                    flags = (C << 3) | (V << 2) | (Z << 1) | (S << 0);
                    FLAG = (ushort)((FLAG & 0) | (ushort)flags);
                    return (short)((dBUS << 1) | copy_C);
                case 15: //rrc
                    rez = dBUS & 0x0001;
                    copy_C = C;
                    C = (ushort)rez;
                    flags = (C << 3) | (V << 2) | (Z << 1) | (S << 0);
                    FLAG = (ushort)((FLAG & 0) | (ushort)flags);
                    return (short)((dBUS >> 1) | copy_C);

                default:

                    throw new Exception("Wrong alu condition!");
            }
        }

        private short GetDbus(long mIR)
        {
            lblDBUS_Content.Background = new SolidColorBrush(Colors.Magenta);
            byte dbus_codification = (byte)((mIR & Masks.MASCA_DBUS) >> Masks.POZITIE_DBUS);
            int indexRD;

            switch (dbus_codification)
            {
                case 0:
                    return 0;
                case 1:
                    pDBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    pIR_DBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pDBUS.Stroke = new SolidColorBrush(Colors.Magenta);

                    int irOffset = IR & 0x00FF;
                    short irOffsetExtended = (short)(((irOffset & 0x0080) >> 7) == 1 ? irOffset | 0xFF00 : irOffset | 0x0000);
                    return irOffsetExtended;
                case 2:
                    pDBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pDBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    pT_DBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    return T;
                case 3:
                    pDBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pDBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    pT_DBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    return (short)(~T);
                case 4:
                    pDBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pDBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    pSP_DBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    return (short)SP;
                case 5:
                    pDBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pDBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    pIVR_DBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    INTR = 0;
                    tbIVR.Text = INTR.ToString();
                    return (short)IVR;
                case 6:
                    pDBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pDBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    pFLAG_DBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    return (short)FLAG;
                case 7:
                    pDBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pDBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    pADR_DBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    return (short)ADR;
                case 8:
                    pDBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pDBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    pMDR_DBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    return MDR;
                case 9:
                    pDBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pDBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    pMDR_DBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    return (short)(~MDR);
                case 10:
                    pDBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pDBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    pPC_DBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    return (short)PC;
                case 11:
                    pDBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pDBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    pRG_DBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    indexRD = IR & 0x000F;
                    tbRG.Text = "R" + indexRD + ": " + RG[indexRD].ToString();
                    registersTBList[indexRD].Background = new SolidColorBrush(Colors.Magenta);
                    return RG[indexRD];
                case 12:
                    pDBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pDBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    pRG_DBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    indexRD = IR & 0x000F;
                    tbRG.Text = "nR" + indexRD + ": " + (~RG[indexRD]).ToString();
                    registersTBList[indexRD].Background = new SolidColorBrush(Colors.Magenta);
                    return (short)(~RG[indexRD]);
                case 13:
                    pDBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pDBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    return 0;

                case 14:
                    pDBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pDBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    return -1;
                case 15:
                    pDBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pDBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    return 1;
                default:
                    throw new Exception("Wrong dbus condition!");
            }
        }

        private short GetSbus(long mIR)
        {
            lblSBUS_Content.Background = new SolidColorBrush(Colors.Magenta);
            byte sbus_codification = (byte)((mIR & Masks.MASCA_SBUS) >> Masks.POZITIE_SBUS);
            int indexRS;

            switch (sbus_codification)
            {
                case 0:
                    return 0;
                case 1:
                    pSBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pIR_SBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pSBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);

                    int irOffset = IR & 0x00FF;
                    short irOffsetExtended = (short)(((irOffset & 0x0080) >> 7) == 1 ? irOffset | 0xFF00 : irOffset | 0x0000);
                    return irOffsetExtended;
                case 2:
                    pSBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pSBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    pT_SBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    return T;
                case 3:
                    pSBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pSBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    pT_SBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    return (short)(~T);
                case 4:
                    pSBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pSBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    pSP_SBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    return (short)SP;
                case 5:
                    pSBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pSBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    pIVR_SBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    INTR = 0; 
                    tbIVR.Text = INTR.ToString();
                    return (short)IVR;
                case 6:
                    pSBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pSBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    pFLAG_SBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    return (short)FLAG;
                case 7:
                    pSBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pSBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    pADR_SBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    return (short)ADR;
                case 8:
                    pSBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pSBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    pMDR_SBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    return MDR;
                case 9:
                    pSBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pSBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    pMDR_SBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    return (short)(~MDR);
                case 10:
                    pSBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pSBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    pPC_SBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    return (short)PC;
                case 11:
                    pSBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pSBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    pRG_SBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    indexRS = (IR & 0x03c0) >> 6;
                    tbRG.Text = "R" + indexRS + ": " + RG[indexRS].ToString();
                    registersTBList[indexRS].Background = new SolidColorBrush(Colors.Magenta);

                    return RG[indexRS];
                case 12:
                    pSBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pSBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    pRG_SBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    indexRS = (IR & 0x03c0) >> 6;
                    tbRG.Text = "nR" + indexRS + ": " + (~RG[indexRS]).ToString();
                    registersTBList[indexRS].Background = new SolidColorBrush(Colors.Magenta);

                    return (short)(~RG[indexRS]);
                case 13:
                    pSBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pSBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    return 0;

                case 14:
                    pSBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pSBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    return -1;
                case 15:
                    pSBUS.Stroke = new SolidColorBrush(Colors.Magenta);
                    pSBUS_ALU.Stroke = new SolidColorBrush(Colors.Magenta);
                    return 1;
                default:
                    throw new Exception("Wrong sbus condition!");
            }
        }

        private int GetIndex(long mIR)
        {
            lblINDEX_Content.Background = new SolidColorBrush(Colors.Magenta);
            byte index = (byte)((mIR & Masks.MASCA_INDEX) >> Masks.POZITIE_INDEX);
            switch (index)
            {
                case 0:
                    return 0;
                case 1:
                    return (byte)((GetCL1(IR) << 1) | GetCL0(IR));
                case 2:
                    return (byte)((IR & Masks.MASCA_MAS) >> Masks.POZITIE_MAS);
                case 3:
                    return (byte)((IR & Masks.MASCA_MAD) >> Masks.POZITIE_MAD);
                case 4:
                    return (byte)((IR & Masks.MASCA_OPCODE_B1) >> Masks.POZITIE_OPCODE_B1);
                case 5:
                    return (byte)((IR & Masks.MASCA_OPCODE_B2) >> Masks.POZITIE_OPCODE_B2);
                case 6:
                    return (byte)((IR & Masks.MASCA_OPCODE_B3) >> Masks.POZITIE_OPCODE_B3);
                case 7:
                    return (byte)((IR & Masks.MASCA_OPCODE_B4) << Masks.POZITIE_OPCODE_B4);
                default:
                    throw new Exception("Index number wrong!");
            }
        }

        private int GetMicroAddress(long mIR)
        {
            lblU_ADR_Content.Background = new SolidColorBrush(Colors.Magenta);
            byte microAddress = (byte)(mIR & Masks.MASCA_MICROADRESA);
            //if go to IFCH
            if (microAddress == 0)
            { instructiuneFinalizata = true; }
            return microAddress;
        }

        private void ResetDisplay(long mIR)
        {
            Color c = Colors.Black;
            c.A = 0; // alpha 0 

            //labels
            lblSBUS_Content.Background = new SolidColorBrush(c);
            lblDBUS_Content.Background = new SolidColorBrush(c);
            lblALU_Content.Background = new SolidColorBrush(c);
            lblRBUS_Content.Background = new SolidColorBrush(c);
            lblOTHER_OP_Content.Background = new SolidColorBrush(c);
            lblMEMORY_OPS_Content.Background = new SolidColorBrush(c);
            lblF_Content.Background = new SolidColorBrush(c);
            lblNTF_Content.Background = new SolidColorBrush(c);
            lblINDEX_Content.Background = new SolidColorBrush(c);
            lblU_ADR_Content.Background = new SolidColorBrush(c);

            //text boxes
            tbFlag.Background = new SolidColorBrush(Colors.White);
            tbADR.Background = new SolidColorBrush(Colors.White);
            tbIR.Background = new SolidColorBrush(Colors.White);
            tbIVR.Background = new SolidColorBrush(Colors.White);
            tbMDR.Background = new SolidColorBrush(Colors.White);
            tbPC.Background = new SolidColorBrush(Colors.White);
            tbRG.Background = new SolidColorBrush(Colors.White);
            tbSP.Background = new SolidColorBrush(Colors.White);
            tbT.Background = new SolidColorBrush(Colors.White);

            //registers
            for (int i = 0; i < registersTBList.Count; i++)
            {
                registersTBList[i].Background = new SolidColorBrush(c);
            }

            //adr paths
            pADR_MEMORY.Stroke = new SolidColorBrush(Colors.Black);
            pADR_DBUS.Stroke = new SolidColorBrush(Colors.Black);
            pADR_SBUS.Stroke = new SolidColorBrush(Colors.Black);
            pRBUS_ADR.Stroke = new SolidColorBrush(Colors.Black);

            //alu paths
            pALU_MUX2.Stroke = new SolidColorBrush(Colors.Black);
            pALU_RBUS.Stroke = new SolidColorBrush(Colors.Black);
            pDBUS_ALU.Stroke = new SolidColorBrush(Colors.Black);
            pSBUS_ALU.Stroke = new SolidColorBrush(Colors.Black);
            pALU_RBUS.Stroke = new SolidColorBrush(Colors.Black);

            //ir paths
            pIR_DBUS.Stroke = new SolidColorBrush(Colors.Black);
            pIR_MUX.Stroke = new SolidColorBrush(Colors.Black);
            pIR_SBUS.Stroke = new SolidColorBrush(Colors.Black);
            pMEMORY_IRMUX.Stroke = new SolidColorBrush(Colors.Black);

            //mux paths
            pMUX2_FLAG.Stroke = new SolidColorBrush(Colors.Black);
            pMUX_MDR.Stroke = new SolidColorBrush(Colors.Black);
            pRBUS_MUX.Stroke = new SolidColorBrush(Colors.Black);
            pRBUS_MUX2.Stroke = new SolidColorBrush(Colors.Black);

            //pc paths
            pPC_DBUS.Stroke = new SolidColorBrush(Colors.Black);
            pPC_SBUS.Stroke = new SolidColorBrush(Colors.Black);
            pRBUS_PC.Stroke = new SolidColorBrush(Colors.Black);
            
            //T paths
            pT_DBUS.Stroke = new SolidColorBrush(Colors.Black);
            pT_SBUS.Stroke = new SolidColorBrush(Colors.Black);

            //SP paths
            pSP_DBUS.Stroke = new SolidColorBrush(Colors.Black);
            pSP_SBUS.Stroke = new SolidColorBrush(Colors.Black);
            pRBUS_SP.Stroke = new SolidColorBrush(Colors.Black);

            pSBUS.Stroke = new SolidColorBrush(Colors.Black);
            pDBUS.Stroke = new SolidColorBrush(Colors.Black);

            //IVR paths
            pIVR_DBUS.Stroke = new SolidColorBrush(Colors.Black);
            pIVR_SBUS.Stroke = new SolidColorBrush(Colors.Black);
            pRBUS_IVR.Stroke = new SolidColorBrush(Colors.Black);

            //MDR paths
            pMDR_DBUS.Stroke = new SolidColorBrush(Colors.Black);
            pMDR_MEMORY.Stroke = new SolidColorBrush(Colors.Black);
            pMDR_SBUS.Stroke = new SolidColorBrush(Colors.Black);

            //Flag paths
            pFLAG_DBUS.Stroke = new SolidColorBrush(Colors.Black);
            pFLAG_SBUS.Stroke = new SolidColorBrush(Colors.Black);

            //RBUS
            pRBUS.Stroke = new SolidColorBrush(Colors.Black);
            pRBUS_T.Stroke = new SolidColorBrush(Colors.Black);
            pRBUS_RG.Stroke = new SolidColorBrush(Colors.Black);

            //RG
            pRG_DBUS.Stroke = new SolidColorBrush(Colors.Black);
            pRG_SBUS.Stroke = new SolidColorBrush(Colors.Black);

            byte sbus_codification = (byte)((mIR & Masks.MASCA_SBUS) >> Masks.POZITIE_SBUS);
            string sbus_s = Convert.ToString(sbus_codification, 2).PadLeft(4, '0');
            lblSBUS_Content.Content = dictionarMicroprogram.sbus.FirstOrDefault(x => x.Value == sbus_s).Key;

            byte dbus_codification = (byte)((mIR & Masks.MASCA_DBUS) >> Masks.POZITIE_DBUS);
            string dbus_s = Convert.ToString(dbus_codification, 2).PadLeft(4, '0');
            lblDBUS_Content.Content = dictionarMicroprogram.sbus.FirstOrDefault(x => x.Value == dbus_s).Key;

            byte alu_codification = (byte)((mIR & Masks.MASCA_ALU) >> Masks.POZITIE_ALU);
            string alu_s = Convert.ToString(alu_codification, 2).PadLeft(4, '0');
            lblALU_Content.Content = dictionarMicroprogram.operatie_Alu.FirstOrDefault(x => x.Value == alu_s).Key;

            byte rbus_codification = (byte)((mIR & Masks.MASCA_RBUS) >> Masks.POZITIE_RBUS);
            string rbus_s = Convert.ToString(rbus_codification, 2).PadLeft(4, '0');
            lblRBUS_Content.Content = dictionarMicroprogram.destRbus.FirstOrDefault(x => x.Value == rbus_s).Key;

            byte otherOps_codification = (byte)((mIR & Masks.MASCA_ALTEOP) >> Masks.POZITIE_ALTEOP);
            string otherOps_s = Convert.ToString(otherOps_codification, 2).PadLeft(5, '0');
            lblOTHER_OP_Content.Content = dictionarMicroprogram.other_op.FirstOrDefault(x => x.Value == otherOps_s).Key;

            byte memory_codification = (byte)((mIR & Masks.MASCA_OPMEMORIE) >> Masks.POZITIE_OPMEMORIE);
            string memory_s = Convert.ToString(memory_codification, 2).PadLeft(2, '0');
            lblMEMORY_OPS_Content.Content = dictionarMicroprogram.memoryOperation.FirstOrDefault(x => x.Value == memory_s).Key;

            byte f = (byte)((mIR & Masks.MASCA_F) >> Masks.POZITIE_F);
            string f_s = Convert.ToString(f, 2).PadLeft(4, '0');
            lblF_Content.Content = dictionarMicroprogram.cod_Ram.FirstOrDefault(x => x.Value == f_s).Key;

            if (f != 0x00){
                byte ntf = (byte)((mIR & Masks.MASCA_NTF) >> Masks.POZITIE_NTF);
                string ntf_s = Convert.ToString(ntf, 2);
                lblNTF_Content.Content = dictionarMicroprogram.nTF.FirstOrDefault(x => x.Value == ntf_s).Key;

                byte index = (byte)((mIR & Masks.MASCA_INDEX) >> Masks.POZITIE_INDEX);
                string index_s = Convert.ToString(index, 2).PadLeft(3, '0');
                lblINDEX_Content.Content = dictionarMicroprogram.index.FirstOrDefault(x => x.Value == index_s).Key;

                byte microAddress = (byte)(mIR & Masks.MASCA_MICROADRESA);
                lblU_ADR_Content.Content = dictionarMicroprogram.etichete.FirstOrDefault(x => x.Value == microAddress).Key;

            }else {
                lblNTF_Content.Content = "-";
                lblINDEX_Content.Content = "-";
                lblU_ADR_Content.Content = "-";

            }
        }

        private byte GetGeneralInstruction(long mIR)
        {
            return (byte)(GetF(mIR) ^ GetNTF(mIR));
        }

        private byte GetNTF(long mIR)
        {
            lblNTF_Content.Background = new SolidColorBrush(Colors.Magenta);
            byte ntf = (byte)((mIR & Masks.MASCA_NTF) >> Masks.POZITIE_NTF);
            return ntf;
        }

        private byte GetF(long mIR)
        {

            lblF_Content.Background = new SolidColorBrush(Colors.Magenta);
            byte f = (byte)((mIR & Masks.MASCA_F) >> Masks.POZITIE_F);
            switch (f)
            {
                case Masks.STEP:
                    return 0;
                case Masks.JUMP:
                    return 1;
                case Masks.AD:
                    return IsDirectAddressingModeToMAD(IR);
                case Masks.INTR:
                    return INTR;
                case Masks.C:
                    return (byte)((FLAG & 0x0008) >> 3);
                case Masks.V:
                    return (byte)((FLAG & 0x0004) >> 2);
                case Masks.Z:
                    return (byte)((FLAG & 0x0002) >> 1);
                case Masks.S:
                    return (byte)((FLAG & 0x0001));
                case Masks.B1:
                    return IsClassB1(IR);

                default:
                    throw new Exception("Something is wrong with function f!");
            }
        }

        private byte IsDirectAddressingModeToMAD(short IRregister)
        {
            if ((IRregister & 0x0030) >> 4 == 1)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        private byte IsClassB1(short IR)
        {

            if (GetCL0(IR) == 0 && GetCL1(IR) == 0)
                return 1;
            else
                return 0;
        }

        private int GetCL0(short IR)
        {
            byte ir15 = (byte)((this.IR & 0x8000) >> 15);
            byte ir13 = (byte)((this.IR & 0x2000) >> 13);
            ir13 = (byte)(~ir13);
            return (ir15 & ir13);
        }

        private int GetCL1(short IR)
        {
            byte ir15 = (byte)((this.IR & 0x8000) >> 15);
            byte ir14 = (byte)((this.IR & 0x4000) >> 14);
            return (ir15 & ir14);
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
        private void BtnViewDecimal_Click(object sender, RoutedEventArgs e)
        {
            tbPC.Text = PC.ToString();
            tbIR.Text = IR.ToString();
            tbFlag.Text = FLAG.ToString();
            tbSP.Text = SP.ToString();

            if (tbRG.Text.Length != 0)
            {

                string[] aux = tbRG.Text.Split(' ');

                int a;
                if (aux[0].Length == 3)
                    a = Convert.ToInt32(tbRG.Text.Substring(1, 1));
                else
                    a = Convert.ToInt32(tbRG.Text.Substring(1, 2));
                tbRG.Text = aux[0] + " " + RG[a].ToString();
            }
            tbT.Text = T.ToString();
            tbIVR.Text = IVR.ToString();
            tbADR.Text = ADR.ToString();
            tbMDR.Text = MDR.ToString();

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

            tbMemoryDisplay.Text = "";
            string aux1 = "";
            for (int j = 0; j < 5000; j++)
            {
                aux1 += j + ": " + MEMORIE[j].ToString();
                aux1 += Environment.NewLine;

            }
            tbMemoryDisplay.Text = aux1;
            tbMPM.Text = "";
            aux1 = "";
            for (int j = 0; j < 173; j++)
            {
                aux1 += j + ": " + MPM[j].ToString().PadLeft(16, '0');
                aux1 += Environment.NewLine;
            }
            tbMPM.Text = aux1;
        }
    }

}
