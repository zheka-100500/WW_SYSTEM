using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM
{
	public static class Extensions
	{
		
		public static void InvokeStaticMethod(this Type type, string methodName, object[] param)
		{
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod;
			MethodInfo method = type.GetMethod(methodName, bindingAttr);
			if (method != null)
			{
				method.Invoke(null, param);
			}
		}
	}
}
