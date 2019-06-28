using System;
using System.IO;
using System.Text;

namespace Bridle.IO
{
	public sealed class FileReader : IDisposable
	{
		private readonly Stream _s;

	    public FileReader(Stream stream, bool isLittleEndian)
	    {
	        _s = stream;
	        SetEndianness(isLittleEndian);
	    }

	    public long Length => _s.Length;

        public FileReader(byte[] file, bool isLittleEndian)
		{
			_s = new MemoryStream(file);
		    SetEndianness(isLittleEndian);
        }

		public FileReader(string fileName, bool isLittleEndian)
		{
			byte[] file = File.ReadAllBytes(fileName);
			_s = new MemoryStream(file);
		    SetEndianness(isLittleEndian);
		}

	    public void SetEndianness(bool isLittleEndian)
	    {
	        if (isLittleEndian)
	        {
	            ReadInt16 = ReadInt16Le;
	            ReadUInt16 = ReadUInt16Le;
                ReadInt32 = ReadInt32Le;
	            ReadUInt32 = ReadUInt32Le;
	            ReadInt64 = ReadInt64Le;
	            ReadUInt64 = ReadUInt64Le;
	            ReadFloat = ReadFloatLe;
	            ReadDouble = ReadDoubleLe;
            }
	        else
	        {
	            ReadInt16 = ReadInt16Be;
	            ReadUInt16 = ReadUInt16Be;
                ReadInt32 = ReadInt32Be;
	            ReadUInt32 = ReadUInt32Be;
	            ReadInt64 = ReadInt64Be;
	            ReadUInt64 = ReadUInt64Be;
	            ReadFloat = ReadFloatBe;
	            ReadDouble = ReadDoubleBe;
            }
	    }

        #region Endianness Agnostic
        public bool ReachedEndOfFile
		{
		    get => _s.Position >= _s.Length;
		}

	    public long Position
		{
			get => _s.Position;
	        set => _s.Seek(value, SeekOrigin.Begin);
	    }

        public byte[] Read(int length)
		{
			byte[] ba = new byte[length];
			_s.Read(ba, 0, length);
			return ba;
		}

	    public byte ReadByte()
	    {
	        return (byte)_s.ReadByte();
	    }

	    public sbyte ReadSByte()
	    {
	        byte value = (byte)_s.ReadByte();
	        if ((value & 0b10000000) == 0)
            {
	            return (sbyte)value;
            }

            return (sbyte)(~value - 1);
        }

        public char ReadChar()
	    {
	        return (char)_s.ReadByte();
	    }

        public bool ReadBool()
	    {
	        return _s.ReadByte() > 0;
	    }
        #endregion

        #region Strings
        public string ReadCString()
	    {
	        StringBuilder sb = new StringBuilder();
	        int c = _s.ReadByte();
	        while (c != 0)
	        {
	            sb.Append((char)c);
	            c = _s.ReadByte();
	        }

	        return sb.ToString();
	    }

	    public string ReadCString(char terminator)
	    {
	        StringBuilder sb = new StringBuilder();
	        int c = _s.ReadByte();
	        while (c != terminator)
	        {
	            sb.Append((char)c);
	            c = _s.ReadByte();
	        }

	        return sb.ToString();
	    }

	    public string ReadCString(uint bufferSize)
	    {
	        StringBuilder sb = new StringBuilder();
	        int i = 1;
	        int c = _s.ReadByte();
	        while (c != 0 && i <= bufferSize)
	        {
	            sb.Append((char)c);
	            c = _s.ReadByte();
	            i++;
	        }

	        _s.Seek(bufferSize - i, SeekOrigin.Current);
	        return sb.ToString();
	    }

	    public string ReadUnterminatedString(uint length)
	    {
	        StringBuilder sb = new StringBuilder((int)length);
	        for (int i = 0; i < length; i++)
	        {
	            sb.Append((char)_s.ReadByte());
	        }

	        return sb.ToString().TrimEnd('\0');
	    }

	    public char[] ReadCharArray(int length)
	    {

	        char[] ba = new char[length];
	        for (int i = 0; i < length; i++)
	        {
	            ba[i] = (char)_s.ReadByte();
	        }

	        return ba;
	    }
        #endregion

        #region Disposal
        public void Dispose()
	    {
	        Dispose(true);
	        GC.SuppressFinalize(this);
	    }

	    ~FileReader()
	    {
	        Dispose(false);
	    }

	    private void Dispose(bool disposing)
	    {
	        if (disposing)
	        {
	            _s.Dispose();
	        }
	    }
        #endregion

        #region Int16
        public delegate short DelegateReadInt16();
	    public DelegateReadInt16 ReadInt16;

        private short ReadInt16Be()
	    {
	        int j = 0;
	        j += _s.ReadByte() << 8;
	        j += _s.ReadByte();
	        return (short)j;
	    }

	    private short ReadInt16Le()
	    {
	        int j = 0;
	        j += _s.ReadByte();
	        j += _s.ReadByte() << 8;
	        return (short)j;
	    }
        #endregion

        #region UInt16
	    public delegate ushort DelegateReadUInt16();
	    public DelegateReadUInt16 ReadUInt16;

        private ushort ReadUInt16Be()
	    {
	        int j = 0;
	        j += _s.ReadByte() << 8;
	        j += _s.ReadByte();
	        return (ushort)j;
	    }

	    private ushort ReadUInt16Le()
	    {
	        int j = 0;
	        j += _s.ReadByte();
	        j += _s.ReadByte() << 8;
	        return (ushort)j;
	    }
        #endregion

        #region Int32
        public delegate int DelegateReadInt32();
	    public DelegateReadInt32 ReadInt32;
        
        private int ReadInt32Be()
		{
			int j = 0;
			j += _s.ReadByte() << 24;
			j += _s.ReadByte() << 16;
			j += _s.ReadByte() << 8;
			j += _s.ReadByte();
			return j;
		}
        
		private int ReadInt32Le()
		{
			int j = 0;
			j += _s.ReadByte();
			j += _s.ReadByte() << 8;
			j += _s.ReadByte() << 16;
			j += _s.ReadByte() << 24;
			return j;
	    }
        #endregion

        #region UInt32
        public delegate uint DelegateReadUInt32();
	    public DelegateReadUInt32 ReadUInt32;

        private uint ReadUInt32Be()
		{
			uint j = 0;
			j += (uint)_s.ReadByte() << 24;
			j += (uint)_s.ReadByte() << 16;
			j += (uint)_s.ReadByte() << 8;
			j += (uint)_s.ReadByte();
			return j;
		}
        
		private uint ReadUInt32Le()
		{
			uint j = 0;
			j += (uint)_s.ReadByte();
			j += (uint)_s.ReadByte() << 8;
			j += (uint)_s.ReadByte() << 16;
			j += (uint)_s.ReadByte() << 24;
			return j;
		}
        #endregion

        #region Int64
	    public delegate long DelegateReadInt64();
	    public DelegateReadInt64 ReadInt64;

	    private long ReadInt64Be()
	    {
	        long j = 0;
	        j += (long)_s.ReadByte() << 56;
	        j += (long)_s.ReadByte() << 48;
	        j += (long)_s.ReadByte() << 40;
	        j += (long)_s.ReadByte() << 32;
	        j += (long)_s.ReadByte() << 24;
	        j += (long)_s.ReadByte() << 16;
	        j += (long)_s.ReadByte() << 8;
	        j += _s.ReadByte();
	        return j;
	    }

	    private long ReadInt64Le()
	    {
	        long j = 0;
	        j += _s.ReadByte();
	        j += (long)_s.ReadByte() << 8;
	        j += (long)_s.ReadByte() << 16;
	        j += (long)_s.ReadByte() << 24;
	        j += (long)_s.ReadByte() << 32;
	        j += (long)_s.ReadByte() << 40;
	        j += (long)_s.ReadByte() << 48;
	        j += (long)_s.ReadByte() << 56;
	        return j;
	    }
        #endregion

        #region UInt64
	    public delegate ulong DelegateReadUInt64();
	    public DelegateReadUInt64 ReadUInt64;

	    private ulong ReadUInt64Be()
	    {
	        ulong j = 0;
	        j += (ulong)_s.ReadByte() << 56;
	        j += (ulong)_s.ReadByte() << 48;
	        j += (ulong)_s.ReadByte() << 40;
	        j += (ulong)_s.ReadByte() << 32;
	        j += (ulong)_s.ReadByte() << 24;
	        j += (ulong)_s.ReadByte() << 16;
	        j += (ulong)_s.ReadByte() << 8;
	        j += (ulong)_s.ReadByte();
	        return j;
	    }

	    private ulong ReadUInt64Le()
	    {
	        ulong j = 0;
	        j += (ulong)_s.ReadByte();
	        j += (ulong)_s.ReadByte() << 8;
	        j += (ulong)_s.ReadByte() << 16;
	        j += (ulong)_s.ReadByte() << 24;
	        j += (ulong)_s.ReadByte() << 32;
	        j += (ulong)_s.ReadByte() << 40;
	        j += (ulong)_s.ReadByte() << 48;
	        j += (ulong)_s.ReadByte() << 56;
	        return j;
        }
        #endregion

	    #region Float
	    public delegate float DelegateReadFloat();
	    public DelegateReadFloat ReadFloat;

	    private float ReadFloatBe()
		{
			byte[] temp = new byte[4];
			_s.Read(temp, 0, 4);
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(temp);
			}

			return BitConverter.ToSingle(temp, 0);
		}
        
		private float ReadFloatLe()
		{
			byte[] temp = new byte[4];
			_s.Read(temp, 0, 4);
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(temp);
			}

			return BitConverter.ToSingle(temp, 0);
	    }
        #endregion

	    #region Double
	    public delegate double DelegateReadDouble();
	    public DelegateReadDouble ReadDouble;

	    private double ReadDoubleBe()
	    {
	        byte[] temp = new byte[8];
	        _s.Read(temp, 0, 8);
	        if (BitConverter.IsLittleEndian)
	        {
	            Array.Reverse(temp);
	        }

	        return BitConverter.ToDouble(temp, 0);
	    }

	    private double ReadDoubleLe()
	    {
	        byte[] temp = new byte[8];
	        _s.Read(temp, 0, 8);
	        if (!BitConverter.IsLittleEndian)
	        {
	            Array.Reverse(temp);
	        }

	        return BitConverter.ToDouble(temp, 0);
        }
	    #endregion
	}
}
