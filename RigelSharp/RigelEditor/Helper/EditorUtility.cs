using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using System.Diagnostics;

using SharpDX;

namespace RigelEditor
{
    public static class EditorUtility
    {
        public static void Dispose(ref IDisposable disposable)
        {
            if (disposable != null) disposable.Dispose();
        }

        public static int SizeOf<T>()
        {
            return System.Runtime.InteropServices.Marshal.SizeOf<T>();
        }

        public static void Log(string info)
        {
            //Console.WriteLine(info);
        }

        public static void Assert(bool condition)
        {
            Debug.Assert(condition);
        }

        public static void Assert(bool condition, string message = null, RigelAssertLevel level = RigelAssertLevel.Fatel)
        {
            if(level >= EditorConfig.AssertLevel)
                Debug.Assert(condition, message);
        }

        #region Dialog

        public static void OpenFileDialog(string folder,string extension = "",Action<string> callback = null)
        {

        }

        public static void OpenFolderDialog(string folder,Action<string> callback)
        {

        }

        public static void SaveFileDialog(string filepath,Action<string> callback)
        {

        }

        #endregion


    }

    public static class VectorExtension
    {
        public static Vector2 Pos(this Vector4 v)
        {
            return new Vector2(v.X, v.Y);
        }
        public static Vector2 Size(this Vector4 v)
        {
            return new Vector2(v.Z, v.W);
        }
    }

}
