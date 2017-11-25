using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace RigelCore
{
    public class FileSystem
    {
        public DirectoryInfo currentDirInfo { get; private set; }

        private DirectoryInfo[] m_subdir;
        private FileInfo[] m_files;

        public DirectoryInfo[] subDirectory
        {
            get
            {
                if(m_subdir == null)
                {
                    if (currentDirInfo.Exists)
                    {
                        m_subdir = currentDirInfo.GetDirectories();
                    }
                }
                return m_subdir;
            }
        }
        public FileInfo[] files
        {
            get
            {
                if(m_files == null)
                {
                    if (currentDirInfo.Exists)
                    {
                        m_files = currentDirInfo.GetFiles();
                    }
                }
                return m_files;
            }
        }

        public FileSystem(string basedir)
        {
            currentDirInfo = new DirectoryInfo(basedir);
        }
    }
}
