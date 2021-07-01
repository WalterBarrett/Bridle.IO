namespace Bridle.IO.MemoryMappedTypes
{
    public class MMByte
    {
        protected readonly FileWriter Writer;
        protected readonly FileReader Reader;
        protected readonly long Offset;

        public MMByte(FileWriter writer, long offset)
        {
            Writer = writer;
            Reader = Writer.GetReader();
            Offset = offset;
        }

        public byte Value
        {
            get => Writer.GetReader().At(Offset, r => r.ReadByte());
            set => Writer[Offset] = value;
        }

        public override string ToString() => $"0x{Value:X2}";
    }
}
