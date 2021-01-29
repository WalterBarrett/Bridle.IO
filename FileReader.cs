using System;
using System.IO;
using System.Text;

namespace Bridle.IO
{
	public sealed class FileReader : FileWrapper, IDisposable
	{
		private BinaryReader _br;

		public FileReader(Stream stream, ByteOrder endianness) : base(stream)
	    {
			_br = new BinaryReader(_s);
	        ByteOrder = endianness;
		}

		public FileReader(string fileName, ByteOrder endianness) : base(new MemoryStream(File.ReadAllBytes(fileName)))
		{
			_br = new BinaryReader(_s);
			ByteOrder = endianness;
		}

		public FileReader(byte[] file, ByteOrder endianness) : base(new MemoryStream(file))
		{
			_br = new BinaryReader(_s);
			ByteOrder = endianness;
		}

        #region Endianness Agnostic
        public byte[] Read(int length) => _br.ReadBytes(length);

		public byte[] Read(long length) // TODO: Optimize
		{
			byte[] ba = new byte[length];
			if (length <= int.MaxValue)
			{
				_s.Read(ba, 0, (int)length);
			}
			else
			{
				for (long i = 0; i < length; i++)
				{
					ba[i] = (byte)_s.ReadByte();
				}
			}
			return ba;
		}

		public byte[] Read(ulong length) // TODO: Optimize
		{
			byte[] ba = new byte[length];
			if (length <= int.MaxValue)
			{
				_s.Read(ba, 0, (int)length);
			}
			else
			{
				for (ulong i = 0; i < length; i++)
				{
					ba[i] = (byte)_s.ReadByte();
				}
			}

			return ba;
		}

		public sbyte[] ReadArraySByte(ulong count) // TODO: Optimize
		{
			sbyte[] arr = new sbyte[count];
			for (ulong i = 0; i < count; i++)
			{
				arr[i] = ReadSByte();
			}

			return arr;
		}

		public char[] ReadArrayChar(ulong count) // TODO: Optimize
		{
			char[] arr = new char[count];
			for (ulong i = 0; i < count; i++)
			{
				arr[i] = ReadChar();
			}

			return arr;
		}

		public bool[] ReadArrayBool(ulong count) // TODO: Optimize
		{
			bool[] arr = new bool[count];
			for (ulong i = 0; i < count; i++)
			{
				arr[i] = ReadBool();
			}

			return arr;
		}

		public string[] ReadArrayCString(ulong count) // TODO: Optimize
		{
			string[] arr = new string[count];
			for (ulong i = 0; i < count; i++)
			{
				arr[i] = ReadCString();
			}

			return arr;
		}

		public short[] ReadArrayInt16(ulong count) // TODO: Optimize
		{
			short[] arr = new short[count];
			for (ulong i = 0; i < count; i++)
			{
				arr[i] = ReadInt16();
			}

			return arr;
		}

		public ushort[] ReadArrayUInt16(ulong count) // TODO: Optimize
		{
            ushort[] arr = new ushort[count];
			for (ulong i = 0; i < count; i++)
			{
				arr[i] = ReadUInt16();
			}

			return arr;
		}

		public int[] ReadArrayInt32(ulong count) // TODO: Optimize
		{
			int[] arr = new int[count];
			for (ulong i = 0; i < count; i++)
			{
				arr[i] = ReadInt32();
			}

			return arr;
		}

		public uint[] ReadArrayUInt32(ulong count) // TODO: Optimize
		{
            uint[] arr = new uint[count];
			for (ulong i = 0; i < count; i++)
			{
				arr[i] = ReadUInt32();
			}

			return arr;
		}

		public long[] ReadArrayInt64(ulong count) // TODO: Optimize
		{
			long[] arr = new long[count];
			for (ulong i = 0; i < count; i++)
			{
				arr[i] = ReadInt64();
			}

			return arr;
		}

		public ulong[] ReadArrayUInt64(ulong count) // TODO: Optimize
		{
            ulong[] arr = new ulong[count];
			for (ulong i = 0; i < count; i++)
			{
				arr[i] = ReadUInt64();
			}

			return arr;
		}

		public float[] ReadArrayFloat(ulong count) // TODO: Optimize
		{
			float[] arr = new float[count];
			for (ulong i = 0; i < count; i++)
			{
				arr[i] = ReadFloat();
			}

			return arr;
		}

		public double[] ReadArrayDouble(ulong count) // TODO: Optimize
		{
			double[] arr = new double[count];
			for (ulong i = 0; i < count; i++)
			{
				arr[i] = ReadDouble();
			}

			return arr;
		}

		public byte ReadByte() => _br.ReadByte();
	    public sbyte ReadSByte() => _br.ReadSByte();
        public char ReadChar() => _br.ReadChar();
		public bool ReadBool() => _br.ReadBoolean();
        #endregion

        #region Strings
        public string ReadCString()
	    {
	        StringBuilder sb = new StringBuilder();
	        int c = _s.ReadByte();
	        while (c != 0 && !ReachedEndOfFile)
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
	        while (c != terminator && !ReachedEndOfFile)
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
	        while (c != 0 && i <= bufferSize && !ReachedEndOfFile)
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
				char c = (char)_s.ReadByte();
				if (c == '\0')
				{
					Position += length - i - 1;
					break;
				}
				else
				{
					sb.Append(c);
				}
	        }

	        return sb.ToString();
	    }

		public string ReadUnterminatedString(uint length, Encoding encoding)
		{
			var a = encoding.GetString(Read(length));
			return a.TrimEnd('\0');
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

        #region Int16/UInt16
        public delegate short DelegateReadInt16();
		public delegate ushort DelegateReadUInt16();
	    public DelegateReadInt16 ReadInt16;
	    public DelegateReadUInt16 ReadUInt16;

        private short ReadInt16Be()
	    {
	        int j = 0;
	        j += _s.ReadByte() << 8;
	        j += _s.ReadByte();
	        return (short)j;
	    }
		
        private ushort ReadUInt16Be()
	    {
	        int j = 0;
	        j |= _s.ReadByte() << 8;
	        j |= _s.ReadByte();
	        return (ushort)j;
	    }
		#endregion

		#region Int32/UInt32
		public delegate int DelegateReadInt32();
		public delegate uint DelegateReadUInt32();
	    public DelegateReadInt32 ReadInt32;
	    public DelegateReadUInt32 ReadUInt32;
        
        private int ReadInt32Be()
		{
			int j = 0;
			j += _s.ReadByte() << 24;
			j += _s.ReadByte() << 16;
			j += _s.ReadByte() << 8;
			j += _s.ReadByte();
			return j;
		}
		
        private uint ReadUInt32Be()
		{
			uint j = 0;
			j |= (uint)_s.ReadByte() << 24;
			j |= (uint)_s.ReadByte() << 16;
			j |= (uint)_s.ReadByte() << 8;
			j |= (uint)_s.ReadByte();
			return j;
		}
		#endregion

		#region Int64/UInt64
		public delegate long DelegateReadInt64();
		public delegate ulong DelegateReadUInt64();
	    public DelegateReadInt64 ReadInt64;
	    public DelegateReadUInt64 ReadUInt64;

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
		#endregion

		#region Float/Double
		public delegate float DelegateReadFloat();
		public delegate double DelegateReadDouble();
	    public DelegateReadFloat ReadFloat;
	    public DelegateReadDouble ReadDouble;

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
	    #endregion

		protected override void SetMethods(ByteOrder byteOrder)
		{
			switch (byteOrder)
			{
				case ByteOrder.LittleEndian:
					ReadInt16 = _br.ReadInt16;
					ReadUInt16 = _br.ReadUInt16;
					ReadInt32 = _br.ReadInt32;
					ReadUInt32 = _br.ReadUInt32;
					ReadInt64 = _br.ReadInt64;
					ReadUInt64 = _br.ReadUInt64;
					ReadFloat = _br.ReadSingle;
					ReadDouble = _br.ReadDouble;
					break;
				case ByteOrder.BigEndian:
					ReadInt16 = ReadInt16Be;
					ReadUInt16 = ReadUInt16Be;
					ReadInt32 = ReadInt32Be;
					ReadUInt32 = ReadUInt32Be;
					ReadInt64 = ReadInt64Be;
					ReadUInt64 = ReadUInt64Be;
					ReadFloat = ReadFloatBe;
					ReadDouble = ReadDoubleBe;
					break;
				case ByteOrder.None:
				default:
					ReadInt16 = null;
					ReadUInt16 = null;
					ReadInt32 = null;
					ReadUInt32 = null;
					ReadInt64 = null;
					ReadUInt64 = null;
					ReadFloat = null;
					ReadDouble = null;
					break;
			}
		}
	}
}
