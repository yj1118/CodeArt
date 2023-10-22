using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web;

namespace CodeArt.MessagePush
{
    public static class Pusher
    {
        /// <summary>
        /// 发送消息给某一个接收人
        /// </summary>
        /// <param name="receiverId"></param>
        /// <param name="type"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        public static void Send(string receiverId, string type, string title, string content)
        {
            DTObject message = DTObject.Create();
            message["type"] = type;
            message["title"] = title;
            message["body"] = content;

            AliPush.PushMessage(message.GetCode(false, false), "ACCOUNT", receiverId, title);
        }

        public static void Send(string receiverId, string type, string title)
        {
            Send(receiverId, type, title, string.Empty);
        }

        public static void SendAsync(DTObject args)
        {
            Task.Run(() =>
            {
                Send(args);
            });
        }

        public static void Send(DTObject args)
        {
            var receiverId = args.GetValue<string>("receiverId", string.Empty);
            var type = args.GetValue<string>("type", string.Empty);
            var title = args.GetValue<string>("title", string.Empty);
            var content = args.GetValue<string>("content", string.Empty);
            Send(receiverId, type, title, content);
        }


        public static void ProxySend(string proxy, string receiverId, string type, string title, string content)
        {
            DTObject args = DTObject.Create();
            args["receiverId"] = receiverId;
            args["type"] = type;
            args["title"] = title;
            args["content"] = content;

            WebUtil.SendPost(proxy, args.ToData());
        }

        public static void ProxySend(string proxy, string receiverId, string type, string title)
        {
            ProxySend(proxy, receiverId, type, title, title);
        }

        public static void ProxySendAsync(string proxy, string receiverId, string type, string title, string content)
        {
            Task.Run(() =>
            {
                ProxySend(proxy, receiverId, type, title, content);
            });
        }
    }
}
