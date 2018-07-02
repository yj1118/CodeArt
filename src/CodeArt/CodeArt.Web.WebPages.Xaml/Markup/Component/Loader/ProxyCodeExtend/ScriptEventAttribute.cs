using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    /// <summary>
    /// 脚本事件，定义了组件可以响应哪些客户端事件来调用自身的脚本行为
    /// 有时候为了形象的表达出组件的事件用途，我们可能会使用select,change等非客户端脚本支持的事件的名称
    /// 因此，可以使用ScriptEventDefine语法来转换，例如：select:click 的意思是，在控件中事件名称为select,客户端调用是触发的click事件
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ScriptEventAttribute : Attribute
    {
        public ScriptEventDefine[] Defines { get; private set; }

        /// <summary>
        /// 通过事件定义代码来指定组件可以响应哪些客户端事件来调用自身的脚本行为
        /// </summary>
        /// <param name="eventDefineCode"></param>
        public ScriptEventAttribute(params string[] defineCodes)
        {
            this.Defines = new ScriptEventDefine[defineCodes.Length];
            for (var i = 0; i < defineCodes.Length; i++)
            {
                this.Defines[i] = new ScriptEventDefine(defineCodes[i]);
            }
        }

        #region 辅助

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ScriptEventAttribute GetAttribute(Type objType)
        {
            var attrs = objType.GetCustomAttributes(typeof(ScriptEventAttribute), true);

            return attrs.Length > 0 ? (attrs[0] as ScriptEventAttribute) : Default;
        }


        public static ScriptEventAttribute Default = new ScriptEventAttribute("click","change");


        #endregion
    }
}
