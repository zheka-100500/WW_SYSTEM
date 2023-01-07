using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WW_SYSTEM.API
{
	public static class Vector3Extensions
	{


		public static Vector ToVector(this Quaternion quaternion)
		{


			return new Vector(quaternion);
		}

		public static Vector ToVector(this Vector3 vector)
		{

			
			return new Vector(vector);
		}
	}
}
