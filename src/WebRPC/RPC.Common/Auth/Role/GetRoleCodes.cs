using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;

namespace RPC.Common
{
    [Procedure("GetRoleCodes")]
    [SafeAccess()]
    public class GetRoleCodes : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getRoleCodes", (g) =>
            {
                
            });

            HandleToCS(data);
            HandleToJS(data);

            data.Transform("!codes");
            return data;
        }

        #region C#

        private void HandleToCS(DTObject data)
        {
            using (var temp = StringPool.Borrow())
            {
                var csRoleCodes = temp.Item;

                csRoleCodes.Append("public static class RoleCodes \r\n");
                csRoleCodes.Append("{ \r\n");

                data.Each("codes", (code) =>
                {
                    csRoleCodes.Append(GetCSItem(code));
                });

                csRoleCodes.Append("} \r\n\r\n");
                
                data.SetValue("CSRoleCodes", csRoleCodes.ToString());
            }
        }

        /*
         * C#版格式：
            /// <summary>
            /// 入微运维 系统管理员
            /// </summary>
            public const string admin_sa = "sa";
         */
        private string GetCSItem(DTObject code)
        {
            using (var temp = StringPool.Borrow())
            {
                var item = temp.Item;

                var label = code.GetValue<string>("label");
                var value = code.GetValue<string>("value");
                var name = code.GetValue<string>("name");

                item.Append("\t/// <summary> \r\n");
                item.Append(string.Format("\t/// {0} \r\n", label));
                item.Append("\t/// <summary> \r\n");

                item.Append(string.Format("\tpublic const string {0} = \"{1}\"; \r\n\r\n", name, value));

                return item.ToString();
            }
        }

        #endregion

        #region JS

        private void HandleToJS(DTObject data)
        {
            using (var temp = StringPool.Borrow())
            {
                var jsRoleCodes = temp.Item;

                jsRoleCodes.Append("export let roleCodes={ \r\n");

                data.Each("codes", (code) =>
                {
                    jsRoleCodes.Append(GetJSItem(code));
                });

                jsRoleCodes.Append("} \r\n\r\n");
                
                data.SetValue("JSRoleCodes", jsRoleCodes.ToString());
            }
        }

        /*
         * JS版格式：
            export let permissionCodes={
              //入微运维 系统管理员
              admin_sa: "sa"
            }
         */
        private string GetJSItem(DTObject code)
        {
            using (var temp = StringPool.Borrow())
            {
                var item = temp.Item;

                var label = code.GetValue<string>("label");
                var value = code.GetValue<string>("value");
                var name = code.GetValue<string>("name");

                item.Append(string.Format("\t// {0} \r\n", label));
                item.Append(string.Format("\t{0}: \"{1}\", \r\n\r\n", name, value));

                return item.ToString();
            }
        }

        #endregion
    }
}


