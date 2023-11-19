using System.IO;

namespace Bridle.IO
{
    public abstract class FileWrapper
    {
        protected readonly Stream _s;

        public long Length => _s.Length;

        protected FileWrapper(Stream stream)
        {
            if (!stream.CanSeek || stream.CanTimeout)
            {
                _s = new MemoryStream();
                stream.CopyTo(_s);
                _s.Position = 0;
            }
            else
            {
                _s = stream;
            }
        }

        public long Position
        {
            get => _s.Position;
            set => _s.Seek(value, SeekOrigin.Begin);
        }

        public bool ReachedEndOfFile => _s.Position >= _s.Length;

        protected abstract void SetMethods(ByteOrder byteOrder);

        private ByteOrder _byteOrder = ByteOrder.None;
        public ByteOrder ByteOrder
        {
            get => _byteOrder;
            set
            {
                if (_byteOrder == value)
                {
                    return;
                }

                _byteOrder = value;
                SetMethods(value);
            }
        }
    }
}
