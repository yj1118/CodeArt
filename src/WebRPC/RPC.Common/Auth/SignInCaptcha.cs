using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using RPC.Common;
using CodeArt.ServiceModel;
using CodeArt;

namespace RPC.Common
{
    [Procedure("SignInCaptcha", typeof(LogExtractor))]
    [SafeAccess()]
    public class SignInCaptcha : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var isPass = TCaptcha.Instance.Verify(arg.GetValue<string>("captcha.ticket", string.Empty), arg.GetValue<string>("captcha.randstr", string.Empty));
            if (isPass)
            {
                return AuthUtil.SignIn(arg);
            }
            else
            {
                throw new Exception("验证码异常");
            }
        }
    }
}