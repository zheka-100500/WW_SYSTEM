using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WW_SYSTEM.API
{
	public static class VectorExtensions
	{
		
		public static Vector3 ToVector3(this Vector vector)
		{
			return new Vector3(vector.x, vector.y, vector.z);
		}

		public static Quaternion ToQuaternion(this Vector vector)
		{
			
			return new Quaternion(vector.x, vector.y, vector.z, Quaternion.identity.w);
		}
	}
}
