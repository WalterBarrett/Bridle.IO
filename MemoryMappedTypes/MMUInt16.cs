namespace Bridle.IO.MemoryMappedTypes
{
    public class MMUInt16
    {
        protected readonly FileWriter Writer;
        protected readonly FileReader Reader;
        protected readonly long Offset;

        public MMUInt16(FileWriter writer, long offset)
        {
            Writer = writer;
            Reader = Writer.GetReader();
            Offset = offset;
        }

        public ushort Value
        {
            get => Reader.At(Offset, r => r.ReadUInt16());
            set => Writer[Offset] = value;
        }

        public override string ToString() => $"0x{Value:X4}";
    }
}
