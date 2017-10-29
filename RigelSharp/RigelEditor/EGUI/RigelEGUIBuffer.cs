using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RigelEditor.EGUI
{
    public class RigelEGUIBuffer<T> where T : struct
    {
        protected const int BUFFER_INIT_SIZE = 1024;
        protected const int BUFFER_EXTEND_TIMES = 2;

        public int BufferSize { get { return BufferData.Length; } }
        public int BufferSizeInByte { get { return BufferSize * ItemSizeInByte; } }
        public int ItemSizeInByte { get { return RigelUtility.SizeOf<T>(); } }
        public bool BufferResized { get; private set; }

        private int m_bufferExtendTimes = BUFFER_EXTEND_TIMES;
        private Action<RigelEGUIBuffer<T>, int> m_extendGenrateFunc = null;

        public int BufferDataCount { get; private set; }
        public bool Dirty { get; private set; }

        internal T[] BufferData;


        public RigelEGUIBuffer(int buffersize = BUFFER_INIT_SIZE, int extendtimes = BUFFER_EXTEND_TIMES, Action<RigelEGUIBuffer<T>, int> extendGenerate = null)
        {
            m_bufferExtendTimes = extendtimes;
            BufferDataCount = 0;
            BufferResized = false;
            m_extendGenrateFunc = extendGenerate;

            BufferData = new T[buffersize];
            if (m_extendGenrateFunc != null) m_extendGenrateFunc.Invoke(this, 0);
        }

        public void IncreaseBufferDataCount(int c = 1)
        {
            BufferDataCount += c;
            Dirty = true;
        }

        public void CheckAndExtendsWithSize(int size)
        {
            if (BufferSize < size)
            {
                int extendpos = BufferSize;

                int newsize = BufferSize * m_bufferExtendTimes;
                while (newsize < size)
                {
                    newsize *= m_bufferExtendTimes;
                }

                Array.Resize(ref BufferData, newsize);
                BufferResized = true;
                if (m_extendGenrateFunc != null) m_extendGenrateFunc.Invoke(this, extendpos);
            }
        }

        public void CheckAndExtends(int tolerance = 16)
        {
            if (BufferDataCount > BufferSize - tolerance)
            {
                int extendpos = BufferSize;
                Array.Resize(ref BufferData, BufferSize * m_bufferExtendTimes);
                BufferResized = true;
                if (m_extendGenrateFunc != null) m_extendGenrateFunc.Invoke(this, extendpos);
            }
        }

        public void SetDirty(bool dirty)
        {
            Dirty = dirty;
        }

        /// <summary>
        /// after recreate the vertexbuffer of graphics api,set this flag to false
        /// </summary>
        public void SetResizeDone()
        {
            BufferResized = false;
        }

        internal void InternalSetBufferDataCount(int count)
        {
            BufferDataCount = count;
            Dirty = true;
        }
    }
}
