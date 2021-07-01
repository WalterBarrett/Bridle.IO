namespace Bridle.IO.MemoryMappedTypes
{
    public class MMUInt32
    {
        protected readonly FileWriter Writer;
        protected readonly FileReader Reader;
        protected readonly long Offset;

        public MMUInt32(FileWriter writer, long offset)
        {
            Writer = writer;
            Reader = Writer.GetReader();
            Offset = offset;
        }

        public uint Value
        {
            get => Reader.At(Offset, r => r.ReadUInt32());
            set => Writer[Offset] = value;
        }

        public override string ToString() => $"0x{Value:X8}";
    }
}
