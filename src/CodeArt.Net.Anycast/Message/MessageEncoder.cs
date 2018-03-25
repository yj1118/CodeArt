using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;

using CodeArt.Log;
using CodeArt.DTO;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;


namespace CodeArt.Net.Anycast
{
    internal sealed class MessageEncoder : MessageToMessageEncoder<Message>
    {
        public MessageEncoder()
        {
        }

        protected override void Encode(IChannelHandlerContext context, Message message, List<object> output)
        {
            IByteBuffer buffer = null;
            try
            {
                buffer = context.Allocator.Buffer();
                byte[] data = message.ToBytes();
                buffer.WriteByte((byte)'F');
                buffer.WriteInt(data.Length);
                buffer.WriteBytes(data);
                output.Add(buffer);
                buffer = null;
            }
            catch (Exception exception)
            {
                throw new CodecException(exception);
            }
            finally
            {
                buffer?.Release();
            }

        }
    }
}
