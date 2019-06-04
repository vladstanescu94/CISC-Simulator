using System;

namespace SimulatorCISC
{
    class Masks
    {
        public const int ADR_START_MEMORIE = 30;

        public enum Mask : long
        {
            SBUS        = 0x0000007800000000,
            DBUS        = 0x0000000780000000,
            ALU         = 0x0000000078000000,
            RBUS        = 0x0000000007800000,
            ALTEOP      = 0x00000000007C0000,
            OPMEMORIE   = 0x0000000000030000,
            NTF         = 0x0000000000000800,
            F           = 0x000000000000F000,
            INDEX       = 0x0000000000000700,
            MICROADRESA = 0x00000000000000FF
        }

        public enum Position {
            SBUS      = 35,
            DBUS      = 31,
            ALU       = 27,
            RBUS      = 23,
            ALTEOP    = 18,
            OPMEMORIE = 16,
            NTF       = 11,
            F         = 12,
            INDEX     = 8
        }

        public enum Ramification : byte {
            STEP = 0,
            JUMP = 1,
            AD   = 2,
            INTR = 3,
            Z    = 4,
            S    = 5,
            C    = 6,
            V    = 7,
            B1   = 8,
            B2   = 9,
            B3   = 10,
            B4   = 11
        }      

        public enum Instructions : int {
            MAS = 0x0C00,
            MAD = 0x0030,

            OPCODE_B1 = 0x7000,
            OPCODE_B2 = 0x03C0,
            OPCODE_B3 = 0x0F00,
            OPCODE_B4 = 0x001F,

            POZITIE_MAS = 9,
            POZITIE_MAD = 3,
            POZITIE_OPCODE_B1 = 11,
            POZITIE_OPCODE_B2 = 5,
            POZITIE_OPCODE_B3 = 7,
            POZITIE_OPCODE_B4 = 2
        }
    }
}
