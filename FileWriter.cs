using System;
using System.IO;

namespace Bridle.IO
{
	public sealed class FileWriter : IDisposable
	{
		private readonly Stream _s;

	    public void Flush()
	    {
            _s.Flush();
	    }

	    private bool _isLittleEndian = false;

        public bool LittleEndian
        {
	        get => _isLittleEndian;
            set
	        {
	            _isLittleEndian = value;
                SetEndianness(value);
            }
	    }


	    public FileWriter(string fileName, bool isLittleEndian, bool overwrite)
	    {
	        if (overwrite)
	        {
	            _s = File.Create(fileName);
            }
            else
	        {
	            _s = File.OpenWrite(fileName);
	        }
            LittleEndian = isLittleEndian;
	    }

	    public FileWriter(Stream stream, bool isLittleEndian)
	    {
            _s = stream;
	        LittleEndian = isLittleEndian;
	    }

	    public long Length => _s.Length;

	    public FileReader GetReader()
	    {
            return new FileReader(_s, LittleEndian);
	    }

	    public void SetEndianness(bool isLittleEndian)
	    {
	        if (isLittleEndian)
	        {
	            WriteInt16 = WriteInt16Le;
	            WriteUInt16 = WriteUInt16Le;
	            WriteInt32 = WriteInt32Le;
	            WriteUInt32 = WriteUInt32Le;
	            WriteInt64 = WriteInt64Le;
	            WriteUInt64 = WriteUInt64Le;
	            WriteFloat = WriteFloatLe;
	            WriteDouble = WriteDoubleLe;
	        }
	        else
	        {
	            WriteInt16 = WriteInt16Be;
	            WriteUInt16 = WriteUInt16Be;
	            WriteInt32 = WriteInt32Be;
	            WriteUInt32 = WriteUInt32Be;
	            WriteInt64 = WriteInt64Be;
	            WriteUInt64 = WriteUInt64Be;
	            WriteFloat = WriteFloatBe;
	            WriteDouble = WriteDoubleBe;
	        }
	    }

        #region Endianness Agnostic
        public Int64 CurrentPosition
	    {
	        get => _s.Position;
	        set => _s.Seek(value, SeekOrigin.Begin);
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
		        byte c = ba[i];
		        _s.WriteByte(c);
		    }
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
    }
}
