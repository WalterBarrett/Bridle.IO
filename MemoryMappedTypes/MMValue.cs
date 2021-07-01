namespace Bridle.IO.MemoryMappedTypes
{
    public class MMValue<T> where T : IReadable, IWritable, new()
    {
        protected readonly FileWriter Writer;
        protected readonly FileReader Reader;
        protected readonly long Offset;

        public MMValue(FileWriter writer, long offset)
        {
            Writer = writer;
            Reader = Writer.GetReader();
            Offset = offset;
        }

        public T Value
        {
            get => Reader.At(Offset, r => r.Read<T>());
            set => Writer[Offset] = value;
        }

        public override string ToString() => Value.ToString();
    }
}
