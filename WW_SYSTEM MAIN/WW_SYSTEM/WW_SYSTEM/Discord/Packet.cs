using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.Discord
{
	public class Packet : IDisposable
	{

		public Packet()
		{
			this.buffer = new List<byte>();
			this.readPos = 0;
		}


		public Packet(int id)
		{
			this.buffer = new List<byte>();
			this.readPos = 0;
			this.Write(id);
		}


		public Packet(byte[] data)
		{
			this.buffer = new List<byte>();
			this.readPos = 0;
			this.SetBytes(data);
		}


		public void SetBytes(byte[] data)
		{
			this.Write(data);
			this.readableBuffer = this.buffer.ToArray();
		}


		public void WriteLength()
		{
			this.buffer.InsertRange(0, BitConverter.GetBytes(this.buffer.Count));
		}


		public void InsertInt(int value)
		{
			this.buffer.InsertRange(0, BitConverter.GetBytes(value));
		}


		public byte[] ToArray()
		{
			this.readableBuffer = this.buffer.ToArray();
			return this.readableBuffer;
		}


		public int Length()
		{
			return this.buffer.Count;
		}


		public int UnreadLength()
		{
			return this.Length() - this.readPos;
		}


		public void Reset(bool shouldReset = true)
		{
			if (shouldReset)
			{
				this.buffer.Clear();
				this.readableBuffer = null;
				this.readPos = 0;
			}
			else
			{
				this.readPos -= 4;
			}
		}


		public void Write(byte value)
		{
			this.buffer.Add(value);
		}


		public void Write(byte[] value)
		{
			this.buffer.AddRange(value);
		}


		public void Write(short value)
		{
			this.buffer.AddRange(BitConverter.GetBytes(value));
		}


		public void Write(int value)
		{
			this.buffer.AddRange(BitConverter.GetBytes(value));
		}


		public void Write(long value)
		{
			this.buffer.AddRange(BitConverter.GetBytes(value));
		}

		public void Write(ulong value)
		{
			this.buffer.AddRange(BitConverter.GetBytes(value));
		}


		public void Write(float value)
		{
			this.buffer.AddRange(BitConverter.GetBytes(value));
		}


		public void Write(bool value)
		{
			this.buffer.AddRange(BitConverter.GetBytes(value));
		}


		public void Write(List<string> Strings)
		{
			Write(Strings.Count);
			foreach (var item in Strings)
			{
				Write(item);
			}
		}

		public List<string> ReadStrings()
		{
			int Count = ReadInt();
			var Result = new List<string>();
			for (int i = 0; i < Count; i++)
			{
				Result.Add(ReadString());

			}
			return Result;
		}

		public void Write(string value, bool ToBase64 = true)
		{
			string d;

			if (ToBase64)
            {
				d = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
			}
            else
            {
				d = value;
            }
	
			this.Write(d.Length);
			this.buffer.AddRange(Encoding.UTF8.GetBytes(d));
		}


		public byte ReadByte(bool moveReadPos = true)
		{
			bool flag = this.buffer.Count > this.readPos;
			if (flag)
			{
				byte result = this.readableBuffer[this.readPos];
				if (moveReadPos)
				{
					this.readPos++;
				}
				return result;
			}
			throw new Exception("Could not read value of type 'byte'!");
		}

		public byte[] GetBytesWithLength()
		{
			int @int = this.ReadInt();
			byte[] array = new byte[@int];
			Buffer.BlockCopy(this.readableBuffer, this.readPos, array, 0, @int);
			this.readPos += @int;
			return array;
		}

		public byte[] ReadBytes(int length, bool moveReadPos = true)
		{
			bool flag = this.buffer.Count > this.readPos;
			if (flag)
			{
				byte[] result = this.buffer.GetRange(this.readPos, length).ToArray();
				if (moveReadPos)
				{
					this.readPos += length;
				}
				return result;
			}
			throw new Exception("Could not read value of type 'byte[]'!");
		}


		public short ReadShort(bool moveReadPos = true)
		{
			bool flag = this.buffer.Count > this.readPos;
			if (flag)
			{
				short result = BitConverter.ToInt16(this.readableBuffer, this.readPos);
				if (moveReadPos)
				{
					this.readPos += 2;
				}
				return result;
			}
			throw new Exception("Could not read value of type 'short'!");
		}


		public int ReadInt(bool moveReadPos = true)
		{
			bool flag = this.buffer.Count > this.readPos;
			if (flag)
			{
				int result = BitConverter.ToInt32(this.readableBuffer, this.readPos);
				if (moveReadPos)
				{
					this.readPos += 4;
				}
				return result;
			}
			throw new Exception("Could not read value of type 'int'!");
		}


		public long ReadLong(bool moveReadPos = true)
		{
			bool flag = this.buffer.Count > this.readPos;
			if (flag)
			{
				long result = BitConverter.ToInt64(this.readableBuffer, this.readPos);
				if (moveReadPos)
				{
					this.readPos += 8;
				}
				return result;
			}
			throw new Exception("Could not read value of type 'long'!");
		}

		public ulong ReadULong(bool moveReadPos = true)
		{
			bool flag = this.buffer.Count > this.readPos;
			if (flag)
			{
				ulong result = (ulong)BitConverter.ToInt64(this.readableBuffer, this.readPos);
				if (moveReadPos)
				{
					this.readPos += 8;
				}
				return result;
			}
			throw new Exception("Could not read value of type 'ulong'!");
		}


		public float ReadFloat(bool moveReadPos = true)
		{
			bool flag = this.buffer.Count > this.readPos;
			if (flag)
			{
				float result = BitConverter.ToSingle(this.readableBuffer, this.readPos);
				if (moveReadPos)
				{
					this.readPos += 4;
				}
				return result;
			}
			throw new Exception("Could not read value of type 'float'!");
		}


		public bool ReadBool(bool moveReadPos = true)
		{
			bool flag = this.buffer.Count > this.readPos;
			if (flag)
			{
				bool result = BitConverter.ToBoolean(this.readableBuffer, this.readPos);
				if (moveReadPos)
				{
					this.readPos++;
				}
				return result;
			}
			throw new Exception("Could not read value of type 'bool'!");
		}

		public string ReadString(bool moveReadPos = true, bool FromBase64 = true)
		{
			string result;
			try
			{
				int num = this.ReadInt(true);
				string @string = Encoding.UTF8.GetString(this.readableBuffer, this.readPos, num);
				bool flag = moveReadPos && @string.Length > 0;
				if (flag)
				{
					this.readPos += num;
				}
                if (FromBase64)
                {
					var b = Convert.FromBase64String(@string);
					result = Encoding.UTF8.GetString(b);
				}
                else
                {
					result = @string;

				}
			
			}
			catch(Exception ex)
			{
				throw new Exception("Could not read value of type 'string'! EX: " + ex.ToString());
			}
			return result;
		}


		protected virtual void Dispose(bool disposing)
		{
			bool flag = !this.disposed;
			if (flag)
			{
				if (disposing)
				{
					this.buffer = null;
					this.readableBuffer = null;
					this.readPos = 0;
				}
				this.disposed = true;
			}
		}


		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}


		private List<byte> buffer;


		private byte[] readableBuffer;


		private int readPos;


		private bool disposed = false;
	}
}
