using System;

namespace Bridle.IO.MemoryMappedTypes
{
    public class MMByteBitfield : MMByte
    {
        public MMByteBitfield(FileWriter file, long offset) : base(file, offset) { }

        public bool Bit01 { get => (Value & 0b00000001) != 0; set => Value = (byte)(value ? Value | 0b00000001 : Value & 0b11111110); }
        public bool Bit02 { get => (Value & 0b00000010) != 0; set => Value = (byte)(value ? Value | 0b00000010 : Value & 0b11111101); }
        public bool Bit04 { get => (Value & 0b00000100) != 0; set => Value = (byte)(value ? Value | 0b00000100 : Value & 0b11111011); }
        public bool Bit08 { get => (Value & 0b00001000) != 0; set => Value = (byte)(value ? Value | 0b00001000 : Value & 0b11110111); }
        public bool Bit10 { get => (Value & 0b00010000) != 0; set => Value = (byte)(value ? Value | 0b00010000 : Value & 0b11101111); }
        public bool Bit20 { get => (Value & 0b00100000) != 0; set => Value = (byte)(value ? Value | 0b00100000 : Value & 0b11011111); }
        public bool Bit40 { get => (Value & 0b01000000) != 0; set => Value = (byte)(value ? Value | 0b01000000 : Value & 0b10111111); }
        public bool Bit80 { get => (Value & 0b10000000) != 0; set => Value = (byte)(value ? Value | 0b10000000 : Value & 0b01111111); }

        public override string ToString() => $"0b{Convert.ToString(Value, 2)}";
    }
}
