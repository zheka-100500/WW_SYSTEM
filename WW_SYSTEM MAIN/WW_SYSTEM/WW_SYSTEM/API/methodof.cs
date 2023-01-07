using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.API
{
    public class methodof<T>
    {
        private readonly MethodInfo method;

        public methodof(T func)
        {
            Delegate del = (Delegate)(object)func;
            this.method = del.Method;
        }

        public static implicit operator methodof<T>(T methodof)
        {
            return new methodof<T>(methodof);
        }

        public static implicit operator MethodInfo(methodof<T> methodof)
        {
            return methodof.method;
        }

        public override string ToString()
        {
            return String.Format(
                "[MethodInfo] {0}.{1}:{2}(..)", method.DeclaringType.Namespace, method.DeclaringType.Name, method.Name);
        }
    }
}
