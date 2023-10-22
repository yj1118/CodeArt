using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt;
using CodeArt.AppSetting;
using CodeArt.DTO;
using CodeArt.Security;
using CodeArt.ServiceModel;
using CodeArt.Web.WebPages;
using CodeArt.WeChat;

namespace RPC.Common
{
    public static class AuthUtil
    {
        public static DTObject SignInByWeChat(dynamic arg)
        {
            return ProcessLogin(arg, "loginByWeChat");
        }

        public static DTObject SignIn(dynamic arg)
        {
            return ProcessLogin(arg, "login");
        }

        public static DTObject SignInOrRegister(dynamic arg)
        {
            return ProcessLogin(arg, "loginOrRegister");
        }

        public static DTObject LoginByVC(dynamic arg)
        {
            return ProcessLogin(arg, "loginByVC");
        }

        public static void ProcessWeChat(dynamic arg)
        {
            if (arg.WXCode != null)
            {
                string openId = WeChatUtil.GetOpenId(arg.WXCode, out int errorCode);
                if (!string.IsNullOrEmpty(openId)) arg["WeChatId"] = openId;
            }
        }


        private static DTObject ProcessLogin(dynamic arg, string invokeName)
        {
            var current = WebPageContext.Current;
            var ip = current.Request.UserHostAddress;
            arg["Ip"] = ip;

            ProcessWeChat(arg);

            DTObject result = ServiceContext.Invoke(invokeName, arg);

            if (!result.Exist("Id")) return result;

            Helper.ProcessImage(result, "Photo.StoreKey", ImageSize._200x200, 2, "Photo", "user");
            var userId = result.GetValue<long>("id");
            var token = CreateToken(userId, IsDark(userId));
            result.Dynamic.Token = token;

            return result;
        }

        public static DTObject RegeisterByVC(dynamic arg)
        {
            ProcessWeChat(arg);

            var result = ServiceContext.InvokeDynamic("addUserByVC", (g) =>
            {
                g.MobileNumber = arg.MobileNumber;
                g.IsDemo = arg.IsDemo;
                g.RealName = arg.RealName;
                g.NickName = arg.NickName;
                g.Password = arg.Password;
                g.Code = arg.Code;
                g.InviterId = arg.InviterId;
                g.WeChatId = arg.WeChatId;
                g.InvitationCode = string.IsNullOrEmpty(arg.InvitationCode) ? null : arg.InvitationCode;
            });

            return AfterRegeister(result);
        }

        public static DTObject RegeisterByName(dynamic arg)
        {
            ProcessWeChat(arg);

            var result = ServiceContext.InvokeDynamic("addUser", (g) =>
            {
                g.AccountName = arg.UserName;
                g.IsDemo = arg.IsDemo;
                g.RealName = arg.RealName;
                g.NickName = arg.NickName;
                g.Password = arg.Password;
                g.InviterId = arg.InviterId;
                g.WeChatId = arg.WeChatId;
                g.InvitationCode = string.IsNullOrEmpty(arg.InvitationCode) ? null : arg.InvitationCode;
            });

            return AfterRegeister(result);
        }

        private static DTObject AfterRegeister(DTObject result)
        {
            Helper.ProcessImage(result, "Photo.StoreKey", ImageSize._200x200, 2, "Photo", "user");

            var userId = result.GetValue<long>("id");

            var token = CreateToken(userId, IsDark(userId));
            result.Dynamic.Token = token;

            return result;
        }

        public static string CreateToken(dynamic userId, bool dark, dynamic tenantId = null)
        {
            var dto = DTObject.Create();
            dto.SetValue("p", userId);
            if (tenantId != null) dto.SetValue("t", tenantId);
            dto.SetValue("d", dark);

            return IdentityToken.Create(dto);
        }

        public static string CreateToken(dynamic userId)
        {
            return CreateToken(userId, IsDark(userId));
        }


        private static string CreateToken(dynamic userId, bool dark, bool ignoreAuth, bool ignoreDark, bool test, dynamic tenantId, DTObject data)
        {
            var dto = DTObject.Create();
            dto.SetValue("p", userId);
            dto.SetValue("d", dark);
            dto.SetValue("ga", ignoreAuth);
            dto.SetValue("gd", ignoreDark);
            dto.SetValue("gt", test);
            if (tenantId != null) dto.SetValue("t", tenantId);
            if(data != null && !data.IsEmpty())
                dto.SetValue("data", data);

            return IdentityToken.Create(dto);
        }


        public static DTObject GetAccountAuth(dynamic userId, dynamic tenantId)
        {
            var data = ServiceContext.InvokeDynamic("getAccountAuth", (g) =>
            {
                g.UserId = userId;
                g.TenantId = tenantId;
            });

            return data;
        }

        //public static string GetPermissionCodes(dynamic userId, dynamic marketerId = null)
        //{
        //    var data = ServiceContext.InvokeDynamic("getUserPermissionCodes", (g) =>
        //    {
        //        g.UserId = userId;
        //        g.TenantId = marketerId;
        //    });

        //    return data.GetValue<string>();
        //}

        //public static string GetRoleCodes(dynamic userId, dynamic marketerId = null)
        //{
        //    var data = ServiceContext.InvokeDynamic("getUserRoleCodes", (g) =>
        //    {
        //        g.UserId = userId;
        //        g.TenantId = marketerId;
        //    });

        //    return data.GetValue<string>();
        //}

        /// <summary>
        /// 验证token，并且将token的数据赋予到回话信息中
        /// </summary>
        public static void VerifyToken()
        {
            VerifyToken((tokenData) =>
            {
                var principalId = tokenData.GetValue<string>("p",string.Empty); //有可能没有登陆者，只是设置了租户
                var tenantId = tokenData.GetValue<long>("t", 0);
                var dark = tokenData.GetValue<bool>("d", false);

                var ignoreAuth = tokenData.GetValue<bool>("ga", false);
                var ignoreDark = tokenData.GetValue<bool>("gd", false);
                var data = tokenData.GetObject("data", DTObject.Empty);

                AppSession.PrincipalId = principalId;
                AppSession.TenantId = tenantId;
                AppSession.Dark = dark || ignoreDark;
                AppSession.IgnoreAuth = ignoreAuth;
                AppSession.Data = data;
            });
        }

        private static void VerifyToken(Action<DTObject> action)
        {
            var context = WebPageContext.Current;

            //安全性验证
            var token = context.Request.Headers["c-token"];
            if (!string.IsNullOrEmpty(token))
            {
                var data = IdentityToken.Verify(token, (newToken) =>
                {
                    if (!string.IsNullOrEmpty(newToken))
                    {
                        context.Response.Headers["c-token"] = newToken; //刷新token
                    }
                });
                action(data);
            }
        }

        private static bool IsDark(dynamic userId, dynamic tenantId = null)
        {
            var arg = DTObject.Create();
            arg["userId"] = userId;
            if (tenantId != null) arg.SetValue("tenantId", tenantId);
            return AppDark.In(arg);
        }

        public static DTObject Enter(dynamic userId)
        {
            return Enter(userId, null, null);
        }

        public static DTObject Enter(dynamic userId, dynamic tenantId)
        {
            return Enter(userId, tenantId, null);
        }

        public static DTObject Enter(dynamic userId, DTObject data)
        {
            return Enter(userId, null, data);
        }

        public static DTObject Enter(dynamic userId, dynamic tenantId, DTObject data)
        {
            if (string.IsNullOrEmpty(userId)) return DTObject.Empty;

            var result = DTObject.Create();
            var dark = IsDark(userId, tenantId);

            result["Dark"] = dark;

            var auth = GetAccountAuth(userId, tenantId);

            result["PermissionCodes"] = auth.PermissionCodes;
            result["RoleCodes"] = auth.RoleCodes;

            result["IgnoreAuth"] = auth.IgnoreAuth;
            result["IgnoreDark"] = auth.IgnoreDark;
            result["Test"] = auth.Test;

            var token = CreateToken(userId, dark, auth.IgnoreAuth, auth.IgnoreDark, auth.Test, tenantId, data);
            result["Token"] = token;

            //补充user信息，很重要，客户端登录时候，需要存储用户信息
            var user = GetUser(userId);
            result["Id"] = user.Id;
            result["Name"] = user.Name;
            result["RealName"] = user.RealName;
            result["MobileNumber"] = user.MobileNumber;

            return result;
        }

        public static DTObject GetUser(dynamic userId)
        {
            var result = ServiceContext.InvokeDynamic("getUser", (g) =>
            {
                g.Id = userId;
                g.Slim = true;
            });

            return result;
        }
    }
}
