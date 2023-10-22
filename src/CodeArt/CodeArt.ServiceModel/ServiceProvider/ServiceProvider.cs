using System;
using System.ServiceModel;
using System.Collections.Generic;

using CodeArt.DTO;
using CodeArt.ServiceModel;
using System.IO;
using CodeArt.IO;
using CodeArt.Concurrent;
using CodeArt.EasyMQ.Event;
using CodeArt.EasyMQ;

namespace CodeArt.ServiceModel
{
    public abstract class ServiceProvider : IServiceProvider
    {
        #region 录制

        public static bool Record
        {
            get;
            set;
        }

        static ServiceProvider()
        {
            Record = ServiceModelConfiguration.Current.Server.Record;
        }

        #endregion

        public DTObject Invoke(ServiceRequest request)
        {
#if DEBUG
            //只有调式模式下才录制
            if (Record)
            {
                var recorder = ServiceRecorderFactory.Create(request);
                var input = request.Argument;
                var output = Invoke(input);
                recorder.Write(request.Name, input, output);
                return output;
            }
#endif
            return Invoke(request.Argument);
        }

        protected virtual DTObject Invoke(DTObject arg)
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

        //public const string OpenServiceRecord = "OpenServiceRecord";

        //public const string CloseServiceRecord = "CloseServiceRecord";

        //#region 事件订阅

        //public static void Subscribe()
        //{
        //    EventPortal.Subscribe(OpenServiceRecord, OpenServiceRecordHandler.Instance);
        //    EventPortal.Subscribe(CloseServiceRecord, CloseServiceRecordHandler.Instance);
        //}

        ///// <summary>
        ///// 取消订阅
        ///// </summary>
        //public static void Cancel()
        //{
        //    EventPortal.Cancel(OpenServiceRecord);
        //    EventPortal.Cancel(CloseServiceRecord);
        //}

        //[SafeAccess]
        //internal class OpenServiceRecordHandler : IEventHandler
        //{
        //    public void Handle(string eventName, TransferData data)
        //    {
        //        var arg = data.Info;
        //        Record = true;
        //    }


        //    public static readonly OpenServiceRecordHandler Instance = new OpenServiceRecordHandler();
        //}

        //[SafeAccess]
        //internal class CloseServiceRecordHandler : IEventHandler
        //{
        //    public void Handle(string eventName, TransferData data)
        //    {
        //        var arg = data.Info;
        //        Record = false;
        //    }


        //    public static readonly CloseServiceRecordHandler Instance = new CloseServiceRecordHandler();
        //}

        //#endregion


    }
}
