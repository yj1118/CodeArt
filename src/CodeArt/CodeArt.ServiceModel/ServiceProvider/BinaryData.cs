using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Runtime.Serialization;

using CodeArt.DTO;

namespace CodeArt.ServiceModel
{
    public struct BinaryData
    {
        public DTObject Info
        {
            get;
            private set;
        }

        public int DataLength
        {
            get;
            private set;
        }

        public byte[] Content
        {
            get;
            private set;
        }


        public BinaryData(DTObject info, int dataLength, byte[] content)
        {
            this.Info = info;
            this.DataLength = dataLength;
            this.Content = content;
        }

        public static readonly BinaryData Empty = new BinaryData(DTObject.Empty, 0, Array.Empty<byte>());

    }
}
