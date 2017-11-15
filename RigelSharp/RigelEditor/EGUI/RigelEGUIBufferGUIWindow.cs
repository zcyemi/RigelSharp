using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RigelEditor.EGUI
{
    internal class RigelEGUIBufferGUIWindow<T> : RigelEGUIBuffer<T> where T : struct
    {
        public RigelEGUIBufferGUIWindow(int buffersize = BUFFER_INIT_SIZE, int extendtimes = BUFFER_EXTEND_TIMES, Action<RigelEGUIBuffer<T>, int> extendGenerate = null) :
            base(buffersize, extendtimes, extendGenerate)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="winbufferinfo"></param>
        /// <param name="newdata"></param>
        /// <returns>return true if buffer has empty block</returns>
        public bool UpdateGUIWindowBuffer(ref RigelEGUIWindowBufferInfo winbufferinfo, List<T> newdata)
        {
            int newdataCount = newdata.Count;
            if (newdataCount == 0) return false;

            bool emptyblock = false;


            if (winbufferinfo.Inited == false)
            {
                winbufferinfo.StartPos = BufferDataCount;
                Array.Copy(
                    newdata.ToArray(),
                    0,
                    BufferData,
                    BufferDataCount,
                    newdataCount
                    );
                IncreaseBufferDataCount(newdataCount);
                winbufferinfo.EndPos = BufferDataCount;

                winbufferinfo.Inited = true;
            }
            else
            {
                if (winbufferinfo.EndPos == BufferDataCount)
                {
                    //buffer at end
                    Array.Copy(
                        newdata.ToArray(),
                        0,
                        BufferData,
                        winbufferinfo.StartPos,
                        newdataCount
                        );
                    InternalSetBufferDataCount(winbufferinfo.StartPos + newdataCount);
                    winbufferinfo.EndPos = BufferDataCount;
                }
                else
                {
                    //buffer not at end
                    if (winbufferinfo.Size != 0) emptyblock = true;

                    winbufferinfo.StartPos = BufferDataCount;
                    Array.Copy(
                        newdata.ToArray(),
                        0,
                        BufferData,
                        BufferDataCount,
                        newdataCount
                        );
                    IncreaseBufferDataCount(newdataCount);
                    winbufferinfo.EndPos = BufferDataCount;
                }
            }


            return emptyblock;
        }

    }
}
