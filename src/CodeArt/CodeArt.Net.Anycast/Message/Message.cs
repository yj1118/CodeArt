using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;

using CodeArt.DTO;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

using CodeArt.IO;

namespace CodeArt.Net.Anycast
{
    public sealed class Message
    {
        public DTObject Header
        {
            get;
            private set;
        }

        public byte[] Body
        {
            get;
            private set;
        }

        public MessageType Type
        {
            get
            {
                return (MessageType)Header.GetValue<byte>(MessageField.MessageType,0);
            }
        }

        private DTObject _bodyDTO;

        public DTObject BodyDTO
        {
            get
            {
                if(_bodyDTO == null)
                {
                    _bodyDTO = DTObject.Create(this.Body);
                }
                return _bodyDTO;
            }
        }

        /// <summary>
        /// 数据包的来源地
        /// </summary>
        public string Origin
        {
            get
            {
                return Header.GetValue<string>(MessageField.Origin);
            }
             set
            {
                Header.SetValue(MessageField.Origin, value);
            }
        }

        public Message(DTObject header, byte[] body)
        {
            this.Header = header;
            this.Body = body;
        }

        public Message Clone()
        {
            return new Message(this.Header.Clone(), this.Body);
        }

        public byte[] ToBytes()
        {
            var headerBytes = this.Header.ToData();
            var messageLength = 2 + headerBytes.Length + 4 + this.Body.Length; //第一个2是headerLength所占字节数、第二个4是BodyLength所占字节数

            byte[] data = null;
            using (var temp = ByteBuffer.Borrow(messageLength))
            {
                var buffer = temp.Item;
                buffer.Write(headerBytes.Length);
                buffer.Write(headerBytes);
                buffer.Write(this.Body.Length);
                buffer.Write(this.Body);
                data = buffer.ToArray();
            }
            return data;
        }

        public static Message FromBytes(byte[] data)
        {
            Message msg = null;
            using (var temp = ByteBuffer.Borrow(data.Length))
            {
                var buffer = temp.Item;
                buffer.Write(data);

                var headerLength = buffer.ReadInt32();
                var headerBytes = buffer.ReadBytes(headerLength);
                var bodyLength = buffer.ReadInt32();
                var bodyBytes = buffer.ReadBytes(bodyLength);

                var header = DTObject.Create(headerBytes);
                msg = new Message(header, bodyBytes);
            }
            return msg;
        }
    }
}
