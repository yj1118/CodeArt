using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Runtime.Serialization;

using CodeArt.DTO;

namespace CodeArt.ServiceModel
{
    public struct BinaryResponse
    {
        public DTObject Info
        {
            get;
            internal set;
        }

        public TransferStatus Status
        {
            get;
            private set;
        }

        public bool IsFirst
        {
            get
            {
                return (this.Status & TransferStatus.First) == TransferStatus.First;
            }
        }

        public bool IsLast
        {
            get
            {
                return (this.Status & TransferStatus.Last) == TransferStatus.Last;
            }
        }


        public byte[] Content
        {
            get;
            private set;
        }


        public BinaryResponse(TransferStatus status, byte[] content)
        {
            this.Info = DTObject.Empty;
            this.Status = status;
            this.Content = content;
        }

        public bool IsEmpty()
        {
            return this.Content.Length == 0;
        }

        public static readonly BinaryResponse Empty = new BinaryResponse(TransferStatus.None, Array.Empty<byte>());
    }


    [FlagsAttribute]
    public enum TransferStatus : byte
    {
        None = 0,
        /// <summary>
        /// 第一次传输
        /// </summary>
        First = 1,
        /// <summary>
        /// 正在传输
        /// </summary>
        Ing = 2,
        /// <summary>
        /// 最后一次传输
        /// </summary>
        Last = 4
    }

}
