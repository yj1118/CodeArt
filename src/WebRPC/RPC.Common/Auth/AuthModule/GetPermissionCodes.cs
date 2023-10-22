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
    [Procedure("GetPermissionCodes")]
    [SafeAccess()]
    public class GetPermissionCodes : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getPermissionCodes", (g) =>
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
                var csPermissionCodes = temp.Item;

                csPermissionCodes.Append("public static class PermissionCodes \r\n");
                csPermissionCodes.Append("{ \r\n");

                data.Each("codes", (code) =>
                {
                    csPermissionCodes.Append(GetCSItem(code));
                });

                csPermissionCodes.Append("} \r\n\r\n");

                data.SetValue("CSPermissionCodes", csPermissionCodes.ToString());
            }
        }

        /*
         * C#版格式：
            /// <summary>
            /// 营销号 目标和关键结果 我的 新增
            /// </summary>
         */
        private string GetCSItem(DTObject code)
        {
            using (var temp = StringPool.Borrow())
            {
                var item = temp.Item;

                var label = code.GetValue<string>("label");
                var value = code.GetValue<string>("value");
                var scope = code.GetValue<string>("scope");
                var name = code.GetValue<string>("name");

                item.Append("\t/// <summary> \r\n");
                item.Append(string.Format("\t/// {0} \r\n", label));
                item.Append("\t/// <summary> \r\n");

                item.Append(string.Format("\tpublic const string {0} = \"{1}\"; \r\n\r\n", name,
                    "{" + string.Format("code:'{0}',scope:'{1}'", value, scope) + "}"));

                return item.ToString();
            }
        }

        #endregion

        #region JS

        private void HandleToJS(DTObject data)
        {
            using (var temp = StringPool.Borrow())
            {
                var jsPermissionCodes = temp.Item;

                jsPermissionCodes.Append("export let permissionCodes={ \r\n");

                data.Each("codes", (code) =>
                {
                    jsPermissionCodes.Append(GetJSItem(code));
                });

                jsPermissionCodes.Append("} \r\n\r\n");

                data.SetValue("JSPermissionCodes", jsPermissionCodes.ToString());
            }
        }

        /*
         * JS版格式：
            export let permissionCodes={
              //营销号 目标和关键结果 我的 查看
              marketer_okr_my_look: {code:"1_2_2",my},
              //营销号 目标和关键结果 我的 新增
              marketer_okr_my_add: {code:"1_2_1",my}
            }
         */
        private string GetJSItem(DTObject code)
        {
            using (var temp = StringPool.Borrow())
            {
                var item = temp.Item;

                var label = code.GetValue<string>("label");
                var value = code.GetValue<string>("value");
                var scope = code.GetValue<string>("scope");
                var name = code.GetValue<string>("name");
                
                item.Append(string.Format("\t// {0} \r\n", label));
                item.Append(string.Format("\t{0}: {1}, \r\n\r\n", name, 
                    "{" + string.Format("c:\"{0}\",s:{1}",value, scope)  +"}"));

                return item.ToString();
            }
        }

        #endregion
    }
}


