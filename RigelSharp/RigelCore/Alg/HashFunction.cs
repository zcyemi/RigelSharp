using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RigelCore.Alg
{
    public class HashFunction
    {
        public static long RSHash(byte[] str)
        {
            int b = 378551;
            int a = 63689;
            long hash = 0;

            int leng = str.Length;
            for (int i = 0; i < leng; i++)
            {
                hash = hash * a + str[i];
                a = a * b;
            }
            return hash;
        }
    }
}
