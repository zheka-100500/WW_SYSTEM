using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WW_SYSTEM.API
{
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
	public class Vector
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
	{
	
		public Vector(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Vector(Vector3 vector)
		{
			this.x = vector.x;
			this.y = vector.y;
			this.z = vector.z;
		}

		public Vector(Quaternion quaternion)
		{
			this.x = quaternion.x;
			this.y = quaternion.y;
			this.z = quaternion.z;
		}

		public static Vector Zero
		{
			get
			{
				return new Vector(0f, 0f, 0f);
			}
		}

	
		public static Vector One
		{
			get
			{
				return new Vector(1f, 1f, 1f);
			}
		}


		public static Vector Forward
		{
			get
			{
				return new Vector(0f, 0f, 1f);
			}
		}


		public static Vector Back
		{
			get
			{
				return new Vector(0f, 0f, -1f);
			}
		}

		
		public static Vector Up
		{
			get
			{
				return new Vector(0f, 1f, 0f);
			}
		}

		
		public static Vector Down
		{
			get
			{
				return new Vector(0f, -1f, 0f);
			}
		}

		public static Vector Right
		{
			get
			{
				return new Vector(1f, 0f, 0f);
			}
		}

	
		public static Vector Left
		{
			get
			{
				return new Vector(-1f, 0f, 0f);
			}
		}

	
		public static float Distance(Vector a, Vector b)
		{
			return (a - b).Magnitude;
		}

		
		public static Vector Lerp(Vector a, Vector b, float t)
		{
			t = Math.Min(t, 1f);
			t = Math.Max(t, 0f);
			return Vector.LerpUnclamped(a, b, t);
		}

	
		public static Vector LerpUnclamped(Vector a, Vector b, float t)
		{
			return a + (b - a) * t;
		}

	
		public static Vector Min(Vector a, Vector b)
		{
			return new Vector(Math.Min(a.x, b.x), Math.Min(a.y, b.y), Math.Min(a.z, b.z));
		}

		
		public static Vector Max(Vector a, Vector b)
		{
			return new Vector(Math.Max(a.x, b.x), Math.Max(a.y, b.y), Math.Max(a.z, b.z));
		}

	
		public float Magnitude
		{
			get
			{
				return (float)Math.Sqrt((double)this.SqrMagnitude);
			}
		}


		public float SqrMagnitude
		{
			get
			{
				return this.x * this.x + this.y * this.y + this.z * this.z;
			}
		}


		public Vector Normalize
		{
			get
			{
				float magnitude = this.Magnitude;
				if ((double)magnitude > 9.99999974737875E-06)
				{
					return this / magnitude;
				}
				return Vector.Zero;
			}
		}


		public static Vector operator +(Vector a, float b)
		{
			return new Vector(a.x + b, a.y + b, a.z + b);
		}

	
		public static Vector operator +(float a, Vector b)
		{
			return b + a;
		}


		public static Vector operator -(Vector a, float b)
		{
			return new Vector(a.x - b, a.y - b, a.z - b);
		}

		
		public static Vector operator *(Vector a, float b)
		{
			return new Vector(a.x * b, a.y * b, a.z * b);
		}

		public static Vector operator *(float a, Vector b)
		{
			return b * a;
		}

	
		public static Vector operator /(Vector a, float b)
		{
			return new Vector(a.x / b, a.y / b, a.z / b);
		}

	
		public static Vector operator +(Vector a, Vector b)
		{
			return new Vector(a.x + b.x, a.y + b.y, a.z + b.z);
		}

	
		public static Vector operator -(Vector a, Vector b)
		{
			return new Vector(a.x - b.x, a.y - b.y, a.z - b.z);
		}


		public static Vector operator *(Vector a, Vector b)
		{
			return new Vector(a.x * b.x, a.y * b.y, a.z * b.z);
		}

	
		public static Vector operator /(Vector a, Vector b)
		{
			return new Vector(a.x / b.x, a.y / b.y, a.z / b.z);
		}

		public static bool operator ==(Vector a, Vector b)
		{
			if (a == null)
			{
				return b == null;
			}
			return b != null && (a.x == b.x && a.y == b.y) && a.z == b.z;
		}

		public static bool operator !=(Vector a, Vector b)
		{
			return !(a == b);
		}


		

	
		public override int GetHashCode()
		{
			return (this.x.GetHashCode() * 397 ^ this.y.GetHashCode()) * 397 ^ this.z.GetHashCode();
		}


		public override string ToString()
		{
			return string.Format("({0}, {1}, {2})", this.x, this.y, this.z);
		}

	
		public string ToString(string format)
		{
			return string.Concat(new string[]
			{
				"(",
				this.x.ToString(format),
				", ",
				this.y.ToString(format),
				", ",
				this.z.ToString(format),
				")"
			});
		}


		public readonly float x;

	
		public readonly float y;

		public readonly float z;
	}
}
