using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.WebPages.Xaml.Script;

namespace CodeArt.Web.XamlControls.Metronic
{
    /// <summary>
    /// 
    /// </summary>
    public class SortableSE : ScriptElement
    {
        public SortableSE() { }

        /// <summary>
        /// 额外的查询参数
        /// </summary>
        public DTObject Parameters
        {
            get
            {
                return this.Metadata.GetObject("paras", DTObject.Empty);
            }
        }

        /// <summary>
        /// 获得查询参数
        /// </summary>
        /// <returns></returns>
        public DTObject GetQuery()
        {
            var arg = DTObject.Create();
            if (!this.Parameters.IsEmpty())
            {
                foreach (var p in this.Parameters.GetDictionary())
                {
                    arg[p.Key] = p.Value;
                }
            }
            return arg;
        }

        /// <summary>
        /// 该属性用于保存数据
        /// </summary>
        public DTObject Value
        {
            get
            {
                return this.Metadata.GetObject("data", DTObject.Empty);
            }
        }
    }
}
