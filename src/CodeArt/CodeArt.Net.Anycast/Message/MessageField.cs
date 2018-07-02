using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Net.Anycast
{
    public static class MessageField
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public const string MessageType = "MT"; //MessageType

        /// <summary>
        /// 登录时提供的身份
        /// </summary>
        public const string LoginIdentity = "LI"; //LoginIdentity

        /// <summary>
        /// 消息的来源地
        /// </summary>
        public const string Origin = "O"; //Origin

        /// <summary>
        /// 多播地址
        /// </summary>
        public const string MulticastAddress = "MA";

        /// <summary>
        /// 目的地地址
        /// </summary>
        public const string Destination = "D";

        /// <summary>
        /// 多个目的地址
        /// </summary>
        public const string Destinations = "Ds";
    }
}
