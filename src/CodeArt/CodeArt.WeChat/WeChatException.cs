using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.WeChat
{
    public class WeChatException : UserUIException
    {
        public int Code
        {
            get;
            private set;
        }

        public WeChatException(DTObject info)
            :base(info.GetValue<string>("errmsg"))
        {
            this.Code = info.GetValue<int>("errcode");
        }

        public static void Check(DTObject info)
        {
            var errcode = info.GetValue<int>("errcode", 0);
            if(errcode > 0)
            {
                throw new WeChatException(info);
            }
        }

        public static bool IsValid(DTObject info)
        {
            var errcode = info.GetValue<int>("errcode", 0);
            if (errcode > 0)
                return false;
            return true;
        }
    }
}
