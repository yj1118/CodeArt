using System.Reflection;
using System.Web;
using System.IO;

using CodeArt.DTO;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages
{
    [SafeAccess]
    public sealed class DTOSender : IResultSender
    {
        private DTOSender() { }

        public void Send(WebPageContext context, object result)
        {
            DTObject dto = result as DTObject;
            if (dto != null)
            {
                context.Response.Write(dto.GetCode(false, false));
                return;
            }
            var code = result as string;
            if (code != null)
            {
                context.Response.Write(code);
                return;
            }
            throw new DTOSenderException("DTOSender对象仅能发送 DTObject 或者 string 类型的对象！当前被发送的对象类型为：" + result.GetType().FullName);
        }

        public void SendError(WebPageContext context, string error)
        {
            var dto = DTObject.Create("{status,message}");
            dto.SetValue("status", "error");
            dto.SetValue("message", error);
            this.Send(context, dto);
        }

        public static readonly IResultSender Instance = new DTOSender();
    }
}
