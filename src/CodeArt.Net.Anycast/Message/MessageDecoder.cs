using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;

using CodeArt.Log;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;

namespace CodeArt.Net.Anycast
{
    internal sealed class MessageDecoder : ByteToMessageDecoder
    {
        public MessageDecoder()
        {
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            if (input.ReadableBytes < 5)
            {
                return;
            }
            input.MarkReaderIndex();

            int magicNumber = input.ReadByte();
            if (magicNumber != 'F')
            {
                input.ResetReaderIndex();
                throw new Exception("Invalid magic number: " + magicNumber);
            }
            int dataLength = input.ReadInt();
            if (input.ReadableBytes < dataLength)
            {
                input.ResetReaderIndex();
                return;
            }
            var decoded = new byte[dataLength];
            input.ReadBytes(decoded);

            output.Add(Message.FromBytes(decoded));
        }
    }
}
