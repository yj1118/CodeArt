using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.Concurrent;
using CodeArt.DTO;
using CodeArt.ModuleNest;
using CodeArt.ServiceModel;

namespace Module.WebUI
{
    [SafeAccess()]
    [ModuleHandler("login")]
    public class Login : ModuleHandlerBase
    {
        public override DTObject Process(DTObject arg)
        {
            var data = ServiceContext.Invoke("login", arg);
            bool success = data.GetValueWithDefault<bool>("success", true);

            if (success)
            {
                MarkLogined(arg, data);
                return Success;
            }
            return Failed;
        }

        private static readonly DTObject Success = DTObject.Create("{success:true}");
        private static readonly DTObject Failed = DTObject.Create("{success:false}");


        /// <summary>
        /// 标示已登录
        /// </summary>
        /// <param name="reponse"></param>
        private void MarkLogined(dynamic arg, dynamic result)
        {
            int photoWidth = arg.GetValue<int>("photoWidth", 80);
            int photoHeight = arg.GetValue<int>("photoHeight", 80);

            Principal.Login(result.Id, result.Name, result.Email);
            if(result.Photo != null) Principal.Photo = ImageUtil.GetDynamicUrl(result.Photo.StoreKey, photoWidth, photoHeight, 2);
            Principal.SetItem("flag", arg.Flag ?? arg.MobileNumber ?? arg.Email); //需要记录用户的登录标识，其他地方需要使用

            if (result.Roles == null)
            {
                Principal.Roles = Array.Empty<Principal.Role>();
            }
            else
            {
                var roles = new List<Principal.Role>(result.Roles.Count);

                foreach (var role in result.Roles)
                {
                    var principalRole = new Principal.Role()
                    {
                        Id = role.Id,
                        MarkedCode = role.MarkedCode
                    };
                    roles.Add(principalRole);
                }
                Principal.Roles = roles.ToArray();
            }

        }
    }
}
