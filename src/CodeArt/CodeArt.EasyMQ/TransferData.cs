using CodeArt.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.EasyMQ
{
    public struct TransferData
    {
        public string Language
        {
            get;
            set;
        }

        public DTObject Info
        {
            get;
            set;
        }

        #region 二进制传输（如果需要的话）

        /// <summary>
        /// 二进制数据的总长度（有可能是分段传输，该值是总数据的长度，而不是本次传输的长度）
        /// </summary>
        public int DataLength
        {
            get;
            private set;
        }

        /// <summary>
        /// 二进制数据
        /// </summary>
        public byte[] Buffer
        {
            get;
            private set;
        }

        #endregion


        public TransferData(string language, DTObject info, int binaryDataLength, byte[] binaryData)
        {
            this.Language = language;
            this.Info = info;
            this.DataLength = binaryDataLength;
            this.Buffer = binaryData;
        }

        public TransferData(string language, DTObject info)
        {
            this.Language = language;
            this.Info = info;
            this.DataLength = 0;
            this.Buffer = Array.Empty<byte>();
        }


        public static TransferData Deserialize(byte[] content)
        {
            using (var temp = ByteBuffer.Borrow(content.Length))
            {
                var source = temp.Item;
                source.Write(content);

                var language = source.ReadString();

                var dtoLength = source.ReadInt32();
                var dtoData = source.ReadBytes(dtoLength);

                DTObject dto = DTObject.Create(dtoData);

                int binaryLength = 0;
                byte[] binaryData = Array.Empty<byte>();

                if (source.ReadPosition < source.Length)
                {
                    binaryLength = source.ReadInt32();
                    var thisTimeLength = source.ReadInt32();
                    binaryData = source.ReadBytes(thisTimeLength);
                }

                return new TransferData(language, dto, binaryLength, binaryData);
            }
        }

        public static byte[] Serialize(TransferData result)
        {
            var size = result.DataLength == 0 ? SegmentSize.Byte512.Value : result.Buffer.Length; 

            using (var temp = ByteBuffer.Borrow(size))
            {
                var target = temp.Item;

                target.Write(result.Language);


                var dtoData = result.Info.ToData();


                target.Write(dtoData.Length);
                target.Write(dtoData);

                if (result.DataLength > 0)
                {
                    target.Write(result.DataLength);
                    target.Write(result.Buffer.Length);
                    target.Write(result.Buffer);
                }

                return target.ToArray();
            }
        }


        public static TransferData CreateEmpty()
        {
            return new TransferData(string.Empty, DTObject.Empty);
        }

    }
}
