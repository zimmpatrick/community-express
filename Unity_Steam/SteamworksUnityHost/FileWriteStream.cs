using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityExpressNS
{
    public class FileWriteStream
    {
        private UInt64 fileHandle;


        internal FileWriteStream(string fileName)
        {
        }

        public bool WriteChunk( byte[] data )
        {
            return WriteChunk(data, data.Length);
        }

        public bool WriteChunk(byte[] data, int length)
        {
            return false;
        }

        public bool Close()
        {
            return false;
        }

        public bool Cancel()
        {
            return false;
        }

        // delgage OnClose();

    }
}
