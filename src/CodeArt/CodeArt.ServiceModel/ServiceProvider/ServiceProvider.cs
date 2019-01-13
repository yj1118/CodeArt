using System;
using System.ServiceModel;
using System.Collections.Generic;

using CodeArt.DTO;
using CodeArt.ServiceModel;
using System.IO;
using CodeArt.IO;

namespace CodeArt.ServiceModel
{
    public abstract class ServiceProvider : IServiceProvider
    {
        public virtual DTObject Invoke(ServiceRequest request)
        {
            return Invoke(request.Argument);
        }

        public virtual DTObject Invoke(DTObject arg)
        {
            return InvokeDynamic(arg);
        }

        protected virtual DTObject InvokeDynamic(dynamic arg)
        {
            return DTObject.Empty;
        }


        public virtual BinaryData InvokeBinary(ServiceRequest request)
        {
            return InvokeBinary(request.Argument, request.TransmittedLength.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="transmittedLength">已经传输了的长度</param>
        /// <returns></returns>
        public BinaryData InvokeBinary(DTObject arg, int transmittedLength)
        {
            var result = InvokeBinary(arg);
            var stream = result.Stream;
            int totalLength = 0;
            byte[] content = null;
            try
            {
                stream.Position = transmittedLength;
                var size = SegmentSize.GetAdviceSize(stream.Length).Value;
                var remain = (int)(stream.Length - stream.Position);
                size = remain > size ? size : remain;

                content = new byte[size];

                stream.Read(content, 0, content.Length);

                totalLength = (int)stream.Length;
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                stream.Dispose();
            }
            return new BinaryData(result.info, totalLength, content);
        }

        protected virtual (DTObject info, Stream Stream) InvokeBinary(DTObject arg)
        {
            return InvokeBinaryDynamic(arg);
        }

        protected virtual (DTObject info, Stream Stream) InvokeBinaryDynamic(dynamic arg)
        {
            return default((DTObject info, Stream Stream));
        }

    }
}
