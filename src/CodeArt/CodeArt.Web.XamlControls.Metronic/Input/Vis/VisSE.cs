using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.Concurrent;
using CodeArt.DTO;
using CodeArt.Web.WebPages.Xaml.Script;


namespace CodeArt.Web.XamlControls.Metronic
{
    public class VisSE : ScriptElement
    {
        public VisSE() { }

        /// <summary>
        /// 设置节点类型，所有新增的节点只能使用指定的类型
        /// </summary>
        /// <param name="values"></param>
        public void NodeTypes(DTObjects values)
        {
            using (var temp = StringPool.Borrow())
            {
                var code = temp.Item;
                code.AppendFormat("{0}.proxy().nodeTypes({1});", this.Id, values.GetCode(false));
                this.View.WriteCode(code.ToString());
            }
        }

        /// <summary>
        /// 设置节点类型，所有新增的节点只能使用指定的类型
        /// </summary>
        public void NodeTypes(string expression)
        {
            using (var temp = StringPool.Borrow())
            {
                var code = temp.Item;
                code.AppendFormat("{0}.proxy().nodeTypes({1});", this.Id, expression);
                this.View.WriteCode(code.ToString());
            }
        }
    }
}
