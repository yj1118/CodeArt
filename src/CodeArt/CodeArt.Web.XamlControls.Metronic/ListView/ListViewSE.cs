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
    public class ListViewSE : ScriptElement
    {
        public ListViewSE() { }

        public int PageSize
        {
            get
            {
                return this.Metadata.GetValue<int>("pageSize");
            }
        }


        /// <summary>
        /// 从1开始的页码
        /// </summary>
        public int PageIndex
        {
            get
            {
                return this.Metadata.GetValue<int>("pageIndex");
            }
        }

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
            arg["pageSize"] = this.PageSize;
            arg["pageIndex"] = this.PageIndex;
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
        /// 加载数据
        /// </summary>
        /// <param name="para">参数</param>
        public void Load(DTObject para)
        {
            this.View.WriteCode(string.Format("{0}.proxy().load({1});", this.Id, para == null ? string.Empty : para.GetCode()));
        }


        /// <summary>
        /// 加载数据
        /// </summary>
        public void Load()
        {
            Load(null);
        }

        public void Reload()
        {
            this.View.WriteCode(string.Format("{0}.proxy().reload();", this.Id));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="senderId">发送命令的按钮，该按钮应该在item区域内</param>
        public void ReloadItem(string senderId)
        {
            this.View.WriteCode(string.Format("{0}.proxy().reloadItem('{1}');", this.Id, senderId));
        }

        public void ReloadItem(string senderId, DTObject data)
        {
            this.View.WriteCode(string.Format("{0}.proxy().reloadItem('{1}',{2});", this.Id, senderId, data.GetCode(false, false)));
        }

        public void ReloadRow(string senderId, DTObject data)
        {
            this.View.WriteCode(string.Format("{0}.proxy().reloadRow('{1}',{2});", this.Id, senderId, data.GetCode(false, false)));
        }

    }
}
