using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading;

using CodeArt.DTO;


namespace CodeArt.Net.Anycast
{
    public struct RtpData
    {
        /// <summary>
        /// 本次接收到数据主体
        /// </summary>
        public byte[] Body
        {
            get;
            private set;
        }

        /// <summary>
        /// 数据的头信息
        /// </summary>
        public DTObject Header
        {
            get;
            private set;
        }

        /// <summary>
        /// 数据的发送者
        /// </summary>
        public Participant Participant
        {
            get;
            private set;
        }

        internal RtpData(Participant participant, DTObject header, byte[] body)
        {
            this.Participant = participant;
            this.Header = header;
            this.Body = body;
        }

        public bool IsEmpty()
        {
            return this.Participant == null;
        }

        public byte[] ToArray()
        {
            return RtpDataAnalyzer.Serialize(this);
        }

        public static RtpData GetData(string origin, byte[] data)
        {
            return RtpDataAnalyzer.Deserialize(origin, data);
        }

        public static readonly RtpData Empty = new RtpData(null, null, null);

    }
}
