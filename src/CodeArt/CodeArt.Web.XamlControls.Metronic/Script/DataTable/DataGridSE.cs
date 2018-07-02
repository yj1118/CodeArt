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
    public class DataGridSE : ScriptElement
    {
        public DataGridSE() { }

        private DTObject[] _selectedItems;

        public DTObject[] SelectedItems
        {
            get
            {
                if (_selectedItems == null)
                {
                    _selectedItems = this.Metadata.GetList("selectedItems").ToArray();
                }
                return _selectedItems;
            }
        }

        private DTObject _query = null;
        private DTObject Query
        {
            get
            {
                if (_query == null)
                {
                    _query = this.Metadata.GetObject("query");
                }
                return _query;
            }
        }


        public int PageSize
        {
            get
            {
                return this.Query == null ? 0 : this.Query.GetValue<int>("pageSize");
            }
        }

        public int PageIndex
        {
            get
            {
                return this.Query == null ? 0 : this.Query.GetValue<int>("pageIndex");
            }
        }

        /// <summary>
        /// 额外的查询参数
        /// </summary>
        public DTObject AdditionalParameters
        {
            get
            {
                return this.Query == null ? DTObject.Empty : this.Query.GetObject("paras");
            }
        }

        #region 发射命令

        /// <summary>
        /// 绑定数据
        /// </summary>
        public void Bind(DTObject data)
        {
            this.View.WriteCode(string.Format("{0}.proxy().bind({1}, {2});", this.Id, data.GetCode(), this.Id));
        }

        public void SelectedItemsIsEmpty()
        {
            this.View.WriteCode(string.Format("{0}.proxy().get().length == 0", this.Id));
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

        #endregion


    }
}
