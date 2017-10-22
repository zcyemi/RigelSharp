using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RigelEditor
{
    public static class RigelUtility
    {
        public static void Dispose(ref IDisposable disposable)
        {
            if (disposable != null) disposable.Dispose();
        }

        public static int SizeOf<T>()
        {
            return System.Runtime.InteropServices.Marshal.SizeOf<T>();
        }
    }
}
