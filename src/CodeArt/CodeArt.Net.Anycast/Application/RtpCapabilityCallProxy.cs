using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using CodeArt.IO;
using CodeArt.Net.Anycast;
using CodeArt.DTO;

namespace CodeArt.Net.Anycast
{
    /// <summary>
    /// 该对象可以从RtpCapability调用中生成的message反向得出相关数据
    /// </summary>
    public class RtpCapabilityCallProxy
    {
        public string Name
        {
            get;
            private set;
        }

        public string EventName
        {
            get;
            private set;
        }

        public RtpData Data
        {
            get;
            private set;
        }

        public string Destination
        {
            get;
            private set;
        }

        public Guid RequestId
        {
            get;
            private set;
        }

        public IEnumerable<string> Destinations
        {
            get;
            private set;
        }

        public RtpCapabilityCallProxy(Message msg)
        {
            this.Name = msg.Header.GetValue<string>(RtpCapability.FieldRtpCapabilityName, string.Empty);
            this.EventName = msg.Header.GetValue<string>(RtpCapability.FieldRtpEventName, string.Empty);
            if (!string.IsNullOrEmpty(this.Name))
            {
                this.Data = RtpDataAnalyzer.Deserialize(msg.Origin, msg.Body);

                var destination = msg.Header.GetValue<string>(MessageField.Destination, string.Empty);
                if (!string.IsNullOrEmpty(destination))
                {
                    this.Destination = destination;
                }

                var destinations = msg.Header.GetList(MessageField.Destinations, false)?.ToArray<string>();
                if (destinations != null)
                {
                    this.Destinations = destinations;
                }

                this.RequestId = msg.Header.GetValue<Guid>(RtpCapability.FieldRtpCallRequestId, Guid.Empty);
            }
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.Name);
        }

        /// <summary>
        /// 创建一个rtp能力可以识别的消息
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="header"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private Message CreateResponseMessage(string eventName, DTObject header, byte[] content,string error,bool isCompleted)
        {
            var rtpData = new RtpData(this.Data.Participant, header, content);
            var data = RtpDataAnalyzer.Serialize(rtpData);
            var msg = RtpCapability.CreateCallMessage(this.Name, eventName, data,this.RequestId, (mh) =>
            {
                if (!string.IsNullOrEmpty(this.Destination))
                {
                    mh.SetValue(MessageField.Destination, this.Destination);
                }
                else if (this.Destinations != null)
                {
                    mh.SetValue(MessageField.Destinations, this.Destinations);
                }

                if (!string.IsNullOrEmpty(error)) mh.SetValue(RtpCapability.FieldRtpCallErrorMessage, error);
                mh.SetValue(RtpCapability.FieldRtpCallIsCompleted, isCompleted);
            });
            msg.Origin = this.Data.Participant.Orgin;
            return msg;
        }

        /// <summary>
        /// 创建一个响应的消息
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public Message CreateResponseMessage(string eventName, DTObject header, bool isCompleted)
        {
            return CreateResponseMessage(eventName, header, Array.Empty<byte>(), null, isCompleted);
        }

        public Message CreateResponseMessage(string eventName, DTObject header, byte[] content, bool isCompleted)
        {
            return CreateResponseMessage(eventName, header, content, null, isCompleted);
        }

        public Message CreateResponseMessage(string eventName, bool isCompleted)
        {
            return CreateResponseMessage(eventName, DTObject.Empty, Array.Empty<byte>(), null, isCompleted);
        }

        public Message CreateResponseMessage(string eventName, string error)
        {
            return CreateResponseMessage(eventName, DTObject.Empty, Array.Empty<byte>(), error, true);
        }
    }
}
