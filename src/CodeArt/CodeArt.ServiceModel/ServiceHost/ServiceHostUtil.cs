using System;
using System.Collections.Generic;
using System.Web;
using System.Text;

using CodeArt.DTO;
using CodeArt.Util;

namespace CodeArt.ServiceModel
{
    public static class ServiceHostUtil
    {
        public static readonly DTObject Success = DTObject.Create("{status:\"success\"}");

        public static DTObject CreateFailed(Exception ex)
        {
            DTObject failed = DTObject.Create();
            failed.SetValue("status", "failed");
            failed.SetValue("message", GetMessage(ex));
            failed.SetValue("userError", ex.IsUserUIException()); //用户错误
            return failed;
        }

        private static string GetMessage(Exception ex)
        {

#if (!DEBUG)
            return ex.GetCompleteMessage();
#endif

#if (DEBUG)
            StringBuilder msg = new StringBuilder();
            msg.AppendLine(ex.GetCompleteMessage());
            msg.Append(ex.GetCompleteStackTrace());
            return msg.ToString();
#endif
        }
    }
}
