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

        public void Send(WebPageContext context, string resultCode)
        {
            //DTObject dto = result as DTObject;
            //if (dto != null)
            //{
            //    context.Response.Write(dto.GetCode(false, false));
            //    return;
            //}
            context.Response.Write(resultCode);
        }

        public void SendError(WebPageContext context, string error)
        {
            var dto = DTObject.Create("{status,message}");
            dto.SetValue("status", "error");
            dto.SetValue("message", error);
            this.Send(context, dto.GetCode(false, false));
        }

        public static readonly IResultSender Instance = new DTOSender();
    }
}
