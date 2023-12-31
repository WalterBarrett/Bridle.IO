﻿using System;
using System.IO;
using System.Runtime.Serialization;

namespace Bridle.IO
{
	public sealed class FileWriter : FileWrapper, IDisposable
	{
		#region Constructors
        public FileWriter(Stream stream, ByteOrder endianness) : base(stream)
		{
			if (!stream.CanWrite)
			{
				throw new ArgumentException($"Can not create a {nameof(FileWriter)} for a stream of type {stream.GetType().FullName}.");
			}

			ByteOrder = endianness;
		}

		public FileWriter(string fileName, ByteOrder endianness, bool overwrite) : base(overwrite ? File.Create(fileName) : File.OpenWrite(fileName))
	    {
			ByteOrder = endianness;
		}

		public FileWriter(byte[] file, ByteOrder endianness) : base(new MemoryStream(file))
		{
			ByteOrder = endianness;
		}

		public FileWriter(ByteOrder endianness) : base(new MemoryStream())
		{
			ByteOrder = endianness;
		}
		#endregion

        public object this[long index]
        {
            set
            {
                long startingPosition = Position;
                Position = index;
                switch (value)
                {
					case byte[] byteArray: Write(byteArray); break;
                    case sbyte[] array: foreach (var v in array) { WriteSByte(v); } break;
                    case ushort[] array: foreach (var v in array) { WriteUInt16(v); } break;
                    case short[] array: foreach (var v in array) { WriteInt16(v); } break;
                    case uint[] array: foreach (var v in array) { WriteUInt32(v); } break;
                    case int[] array: foreach (var v in array) { WriteInt32(v); } break;
                    case ulong[] array: foreach (var v in array) { WriteUInt64(v); } break;
                    case long[] array: foreach (var v in array) { WriteInt64(v); } break;
                    case float[] array: foreach (var v in array) { WriteFloat(v); } break;
                    case double[] array: foreach (var v in array) { WriteDouble(v); } break;
                    case char[] array: foreach (var v in array) { WriteChar(v); } break;
                    case bool[] array: foreach (var v in array) { WriteBool(v); } break;
                    case string[] array: foreach (var v in array) { WriteCString(v); } break;
                    case IWritable[] array: foreach (var v in array) { v.Write(this); } break;

					case byte v: WriteByte(v); break;
                    case sbyte v: WriteSByte(v); break;
                    case ushort v: WriteUInt16(v);  break;
                    case short v: WriteInt16(v); break;
                    case uint v: WriteUInt32(v); break;
                    case int v: WriteInt32(v); break;
                    case ulong v: WriteUInt64(v); break;
                    case long v: WriteInt64(v); break;
                    case float v: WriteFloat(v); break;
                    case double v: WriteDouble(v); break;
                    case char v: WriteChar(v); break;
                    case bool v: WriteBool(v); break;
                    case string v: WriteCString(v); break;
                    case IWritable v: v.Write(this); break;

                    default: throw new NotImplementedException();
				}

                Position = startingPosition;
            }
        }

		#region Endianness Agnostic
		public byte[] ToArray()
		{
			var oldPos = _s.Position;
			_s.Position = 0;
			byte[] ret = new byte[_s.Length];
			_s.Read(ret, 0, (int)_s.Length);
			_s.Position = oldPos;
			return ret;
		}

		public void WriteCString(string value, int forceLength = -1)
        {
            int i;

            if (forceLength == -1)
            {
                forceLength = (value?.Length ?? 0) + 1;
            }

            if (value == null)
			{
				_s.WriteByte(0x00);
			    i = 1;
			}
			else
			{
		        for (i = 0; i < value.Length && i < forceLength - 1; i++)
		        {
		            char c = value[i];
		            _s.WriteByte((byte)c);
                }
            }

            for (; i < forceLength; i++)
            {
                _s.WriteByte(0x00);
            }
	    }

	    public void WriteUnterminatedString(string value, int forceLength = -1)
	    {
			if (value == null)
			{
				return;
			}

	        int i = 0;

	        if (forceLength == -1)
	        {
	            forceLength = value.Length;
	        }

	        if (value != null)
	        {
	            for (i = 0; i < value.Length; i++)
	            {
	                char c = value[i];
	                _s.WriteByte((byte)c);
	            }
	        }

	        for (; i < forceLength; i++)
	        {
	            _s.WriteByte(0x00);
	        }
        }

        public void Write(byte[] ba)
		{
		    if (ba == null)
			{
				return;
			}

		    for (int i = 0; i < ba.Length; i++)
		    {
		        _s.WriteByte(ba[i]);
		    }
		}

		public void Write(FileReader fr)
		{
			if (fr == null)
			{
				return;
			}

			byte[] ba = fr.ToArray();
			_s.Write(ba, 0, ba.Length);
		}

		public void WriteByte(byte value)
	    {
	        _s.WriteByte(value);
	    }

	    public void WriteSByte(sbyte value)
	    {
	        _s.Write(BitConverter.GetBytes(value), 0, 1);
        }

        public void WriteChar(char value)
	    {
	        _s.WriteByte((byte)value);
	    }

        public void WriteBool(bool value)
	    {
	        _s.WriteByte(value ? (byte)1 : (byte)0);
        }
        #endregion

        #region Disposal
        public void Dispose()
	    {
	        Dispose(true);
	        GC.SuppressFinalize(this);
	    }

	    ~FileWriter()
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
	    public delegate void DelegateWriteInt16(short value);
	    public DelegateWriteInt16 WriteInt16;

	    private void WriteInt16Be(short value)
	    {
	        /*_s.WriteByte((byte)((value & 0xFF00) >> 8));
            _s.WriteByte((byte)(value & 0xFF));*/
	        byte[] temp = BitConverter.GetBytes(value);
	        if (BitConverter.IsLittleEndian)
	        {
	            Array.Reverse(temp);
	        }
	        _s.Write(temp, 0, temp.Length);
	    }

	    private void WriteInt16Le(short value)
	    {
	        /*_s.WriteByte((byte)(value & 0xFF));
            _s.WriteByte((byte)((value & 0xFF00) >> 8));*/
	        byte[] temp = BitConverter.GetBytes(value);
	        if (!BitConverter.IsLittleEndian)
	        {
	            Array.Reverse(temp);
	        }
	        _s.Write(temp, 0, temp.Length);
	    }
	    #endregion

        #region Int32
        public delegate void DelegateWriteInt32(int value);
	    public DelegateWriteInt32 WriteInt32;

        private void WriteInt32Be(int value)
		{
			/*_s.WriteByte((byte)((value & 0xFF000000) >> 24));
			_s.WriteByte((byte)((value & 0xFF0000) >> 16));
			_s.WriteByte((byte)((value & 0xFF00) >> 8));
			_s.WriteByte((byte)(value & 0xFF));*/
		    byte[] temp = BitConverter.GetBytes(value);
		    if (BitConverter.IsLittleEndian)
		    {
		        Array.Reverse(temp);
		    }
		    _s.Write(temp, 0, temp.Length);
        }

	    private void WriteInt32Le(int value)
		{
			/*_s.WriteByte((byte)(value & 0xFF));
			_s.WriteByte((byte)((value & 0xFF00) >> 8));
			_s.WriteByte((byte)((value & 0xFF0000) >> 16));
			_s.WriteByte((byte)((value & 0xFF000000) >> 24));*/
		    byte[] temp = BitConverter.GetBytes(value);
		    if (!BitConverter.IsLittleEndian)
		    {
		        Array.Reverse(temp);
		    }
		    _s.Write(temp, 0, temp.Length);
        }
        #endregion

	    #region Int64
	    public delegate void DelegateWriteInt64(long value);
	    public DelegateWriteInt64 WriteInt64;

	    private void WriteInt64Be(long value)
	    {
	        byte[] temp = BitConverter.GetBytes(value);
	        if (BitConverter.IsLittleEndian)
	        {
	            Array.Reverse(temp);
	        }
	        _s.Write(temp, 0, temp.Length);
	    }

	    private void WriteInt64Le(long value)
	    {
	        byte[] temp = BitConverter.GetBytes(value);
	        if (!BitConverter.IsLittleEndian)
	        {
	            Array.Reverse(temp);
	        }
	        _s.Write(temp, 0, temp.Length);
	    }
	    #endregion

        #region UInt32
        public delegate void DelegateWriteUInt32(uint value);
	    public DelegateWriteUInt32 WriteUInt32;

	    private void WriteUInt32Be(uint value)
		{
			_s.WriteByte((byte)((value & 0xFF000000) >> 24));
			_s.WriteByte((byte)((value & 0xFF0000) >> 16));
			_s.WriteByte((byte)((value & 0xFF00) >> 8));
			_s.WriteByte((byte)(value & 0xFF));
		}
        
		private void WriteUInt32Le(uint value)
		{
			_s.WriteByte((byte)(value & 0xFF));
			_s.WriteByte((byte)((value & 0xFF00) >> 8));
			_s.WriteByte((byte)((value & 0xFF0000) >> 16));
			_s.WriteByte((byte)((value & 0xFF000000) >> 24));
	    }
        #endregion

        #region UInt16
        public delegate void DelegateWriteUInt16(ushort value);
	    public DelegateWriteUInt16 WriteUInt16;

	    private void WriteUInt16Be(ushort value)
		{
			_s.WriteByte((byte)((value & 0xFF00) >> 8));
			_s.WriteByte((byte)(value & 0xFF));
		}

	    private void WriteUInt16Le(ushort value)
		{
			_s.WriteByte((byte)(value & 0xFF));
			_s.WriteByte((byte)((value & 0xFF00) >> 8));
	    }
        #endregion

	    #region UInt64
        public delegate void DelegateWriteUInt64(ulong value);
	    public DelegateWriteUInt64 WriteUInt64;

	    private void WriteUInt64Be(ulong value)
		{
			_s.WriteByte((byte)((value & 0xFF00000000000000) >> 56));
			_s.WriteByte((byte)((value & 0xFF000000000000) >> 48));
			_s.WriteByte((byte)((value & 0xFF0000000000) >> 40));
			_s.WriteByte((byte)((value & 0xFF00000000) >> 32));
			_s.WriteByte((byte)((value & 0xFF000000) >> 24));
			_s.WriteByte((byte)((value & 0xFF0000) >> 16));
			_s.WriteByte((byte)((value & 0xFF00) >> 8));
			_s.WriteByte((byte)(value & 0xFF));
		}

	    private void WriteUInt64Le(ulong value)
		{
			_s.WriteByte((byte)(value & 0xFF));
			_s.WriteByte((byte)((value & 0xFF00) >> 8));
			_s.WriteByte((byte)((value & 0xFF0000) >> 16));
			_s.WriteByte((byte)((value & 0xFF000000) >> 24));
			_s.WriteByte((byte)((value & 0xFF00000000) >> 32));
			_s.WriteByte((byte)((value & 0xFF0000000000) >> 40));
			_s.WriteByte((byte)((value & 0xFF000000000000) >> 48));
			_s.WriteByte((byte)((value & 0xFF00000000000000) >> 56));
	    }
        #endregion

        #region Float
        public delegate void DelegateWriteFloat(float value);
        public DelegateWriteFloat WriteFloat;

        private void WriteFloatBe(float value)
        {
            byte[] temp = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(temp);
            }
            _s.Write(temp, 0, temp.Length);
        }

        private void WriteFloatLe(float value)
        {
            byte[] temp = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(temp);
            }
            _s.Write(temp, 0, temp.Length);
        }
        #endregion

        #region Double
        public delegate void DelegateWriteDouble(double value);
        public DelegateWriteDouble WriteDouble;

        private void WriteDoubleBe(double value)
        {
            byte[] temp = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(temp);
            }
            _s.Write(temp, 0, temp.Length);
        }

        private void WriteDoubleLe(double value)
        {
            byte[] temp = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(temp);
            }
            _s.Write(temp, 0, temp.Length);
        }
		#endregion

		#region Strings

		#endregion

		protected override void SetMethods(ByteOrder byteOrder)
		{
			switch (byteOrder)
			{
				case ByteOrder.LittleEndian:
					WriteInt16 = WriteInt16Le;
					WriteUInt16 = WriteUInt16Le;
					WriteInt32 = WriteInt32Le;
					WriteUInt32 = WriteUInt32Le;
					WriteInt64 = WriteInt64Le;
					WriteUInt64 = WriteUInt64Le;
					WriteFloat = WriteFloatLe;
					WriteDouble = WriteDoubleLe;
					break;
				case ByteOrder.BigEndian:
					WriteInt16 = WriteInt16Be;
					WriteUInt16 = WriteUInt16Be;
					WriteInt32 = WriteInt32Be;
					WriteUInt32 = WriteUInt32Be;
					WriteInt64 = WriteInt64Be;
					WriteUInt64 = WriteUInt64Be;
					WriteFloat = WriteFloatBe;
					WriteDouble = WriteDoubleBe;
					break;
				case ByteOrder.None:
				default:
					WriteInt16 = null;
					WriteUInt16 = null;
					WriteInt32 = null;
					WriteUInt32 = null;
					WriteInt64 = null;
					WriteUInt64 = null;
					WriteFloat = null;
					WriteDouble = null;
					break;
			}
		}

        [Obsolete("GetReader() is deprecated due to possible disposal issues; please use ToArray() instead.")]
        public FileReader GetReader() => new FileReader(_s, ByteOrder);

        public void Flush()
		{
			_s.Flush();
        }

        public void Write<T>(T value) where T : IWritable, new() => value.Write(this);
        public void WriteArray<T>(T[] array) where T : IWritable, new() { foreach (T v in array) { Write(v); } }
        public void WriteArraySByte(sbyte[] array) { foreach (sbyte v in array) { WriteSByte(v); } }
        public void WriteArrayChar(char[] array) { foreach (char v in array) { WriteChar(v); } }
        public void WriteArrayBool(bool[] array) { foreach (bool v in array) { WriteBool(v); } }
        public void WriteArrayCString(string[] array) { foreach (string v in array) { WriteCString(v); } }
        public void WriteArrayUInt16(ushort[] array) { foreach (ushort v in array) { WriteUInt16(v); } }
        public void WriteArrayInt16(short[] array) { foreach (short v in array) { WriteInt16(v); } }
        public void WriteArrayUInt32(uint[] array) { foreach (uint v in array) { WriteUInt32(v); } }
        public void WriteArrayInt32(int[] array) { foreach (int v in array) { WriteInt32(v); } }
        public void WriteArrayUInt64(ulong[] array) { foreach (ulong v in array) { WriteUInt64(v); } }
        public void WriteArrayInt64(long[] array) { foreach (long v in array) { WriteInt64(v); } }
        public void WriteArrayFloat(float[] array) { foreach (float v in array) { WriteFloat(v); } }
        public void WriteArrayDouble(double[] array) { foreach (double v in array) { WriteDouble(v); } }
    }
}
