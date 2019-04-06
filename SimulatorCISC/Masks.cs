using System;

namespace SimulatorCISC
{
    class Masks
    {
        //Adresa de la care incep instructiunile in memorie
        public const int ADR_START_MEMORIE = 20;

        // MIR
        public const Int64 MASCA_SBUS = 0x0000007800000000;
        public const int POZITIE_SBUS = 35;
        public const Int64 MASCA_DBUS = 0x0000000780000000;
        public const int POZITIE_DBUS = 31;
        public const Int64 MASCA_ALU = 0x0000000078000000;
        public const int POZITIE_ALU = 27;
        public const Int64 MASCA_RBUS = 0x0000000007800000;
        public const int POZITIE_RBUS = 23;
        public const Int64 MASCA_ALTEOP = 0x00000000007C0000;
        public const int POZITIE_ALTEOP = 18;
        public const Int64 MASCA_OPMEMORIE = 0x0000000000030000;
        public const int POZITIE_OPMEMORIE = 16;
        public const Int64 MASCA_NTF = 0x0000000000000800;
        public const int POZITIE_NTF = 11;
        public const Int64 MASCA_F = 0x000000000000F000;
        public const int POZITIE_F = 12;
        public const Int64 MASCA_INDEX = 0x0000000000000700;
        public const int POZITIE_INDEX = 8;
        public const Int64 MASCA_MICROADRESA = 0x00000000000000FF;


        public const byte STEP = 0;
        public const byte JUMP = 1;
        public const byte AD = 2;
        public const byte INTR = 3;
        public const byte Z = 4;
        public const byte S = 5;
        public const byte C = 6;
        public const byte V = 7;
        public const byte B1 = 8;
        public const byte B2 = 9;
        public const byte B3 = 10;
        public const byte B4 = 11;

        //index

        public const UInt16 MASCA_MAS = 0x0C00;
        public const int POZITIE_MAS = 9;
        public const UInt16 MASCA_MAD = 0x0030;
        public const int POZITIE_MAD = 3;
        public const UInt16 MASCA_OPCODE_B1 = 0x7000;
        public const int POZITIE_OPCODE_B1 = 11;
        public const UInt16 MASCA_OPCODE_B2 = 0x03C0;
        public const int POZITIE_OPCODE_B2 = 5;
        public const UInt16 MASCA_OPCODE_B3 = 0x0F00;
        public const int POZITIE_OPCODE_B3 = 7;
        public const UInt16 MASCA_OPCODE_B4 = 0x001F;
        public const int POZITIE_OPCODE_B4 = 2;
    }
}
