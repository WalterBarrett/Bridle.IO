namespace Bridle.IO.MemoryMappedTypes
{
    public class MMUInt64
    {
        protected readonly FileWriter Writer;
        protected readonly FileReader Reader;
        protected readonly long Offset;

        public MMUInt64(FileWriter writer, long offset)
        {
            Writer = writer;
            Reader = Writer.GetReader();
            Offset = offset;
        }

        public ulong Value
        {
            get => Reader.At(Offset, r => r.ReadUInt64());
            set => Writer[Offset] = value;
        }

        public override string ToString() => $"0x{Value:X16}";
    }
}
