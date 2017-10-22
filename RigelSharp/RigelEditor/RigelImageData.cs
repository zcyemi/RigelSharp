using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing.Imaging;
using System.Drawing;

namespace RigelEditor
{
    public enum RigelImageDataDepth
    {
        Depth32 = 4,
        Depth24 = 3,
    };

    public class RigelImageData:IDisposable
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public byte[] Data { get; private set; }
        public RigelImageDataDepth Depth { get; private set; }
        public int DataSize { get { return Data.Length; } }
        public int PixelSize { get { return (int)Depth; } }
        public int Pitch { get { return Width * PixelSize; } }


        public RigelImageData(int width,int height, RigelImageDataDepth depth = RigelImageDataDepth.Depth32)
        {
            Width = width;
            Height = height;
            Depth = depth;
            Data = new byte[width * height *(int)depth];
        }

        public void Dispose()
        {
            
        }

        public PixelFormat GetPixelFormat()
        {
            if(Depth == RigelImageDataDepth.Depth32)
            {
                return PixelFormat.Format32bppArgb;
            }
            else
            {
                return PixelFormat.Format24bppRgb;
            }
        }

        public void SaveToFile(string filepath,ImageFormat imgfmt)
        {
            Bitmap bmp = new Bitmap(Width, Height,GetPixelFormat());
            
            var bmpdata = bmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, GetPixelFormat());

            Console.WriteLine(Data.Length);
            System.Runtime.InteropServices.Marshal.Copy(Data, 0, bmpdata.Scan0, Data.Length);

            bmp.UnlockBits(bmpdata);

            bmp.Save(filepath, imgfmt);
        }
    }
}
