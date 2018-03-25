using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.Net.Anycast
{
    public sealed class CertifiedResult
    {
        /// <summary>
        /// 是否通过
        /// </summary>
        public bool IsOK
        {
            get;
            private set;
        }

        /// <summary>
        /// 附带的信息
        /// </summary>
        public string Info
        {
            get;
            private set;
        }

        public CertifiedResult(bool isOK,string info)
        {
            this.IsOK = isOK;
            this.Info = info;
        }

        internal Message ToMessage()
        {
            var header = DTObject.Create();
            header.SetValue(MessageField.MessageType, MessageType.LoginResponse);
            header.SetValue("IK", this.IsOK ? 1 : 0);
            header.SetValue("I", this.Info);

            return new Message(header, Array.Empty<byte>());
        }

        internal static CertifiedResult FromMessage(Message msg)
        {
            var isOK = msg.Header.GetValue<int>("IK") == 1;
            var info = msg.Header.GetValue<string>("I");
            return new CertifiedResult(isOK, info);
        }


        public static readonly CertifiedResult Success = new CertifiedResult(true, string.Empty);

    }
}
