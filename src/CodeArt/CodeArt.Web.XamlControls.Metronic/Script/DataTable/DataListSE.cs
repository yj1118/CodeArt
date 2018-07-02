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
    public class DataListSE : ScriptElement
    {
        public DataListSE() { }

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

        private DTObject[] _data;

        public DTObject[] Data
        {
            get
            {
                if (_data == null)
                {
                    _data = this.Metadata.GetList("data").ToArray();
                }
                return _data;
            }
        }


        #region 发射命令


        /// <summary>
        /// 绑定数据
        /// </summary>
        public void Bind(DTObject data)
        {
            this.View.WriteCode(string.Format("{0}.proxy().bind({1});", this.Id, data.GetCode()));
        }

        /// <summary>
        /// 绑定数据，并提供查询条件，这在开启url同步模式时使用
        /// </summary>
        /// <param name="data"></param>
        /// <param name="paras"></param>
        public void Bind(DTObject data,DTObject paras)
        {
            this.View.WriteCode(string.Format("{0}.proxy().bind({1},{2});", this.Id, data.GetCode(), paras.GetCode()));
        }

        public void SelectedItemsIsEmpty()
        {
            this.View.WriteCode(string.Format("{0}.proxy().get().length == 0", this.Id));
        }

        public void Reset()
        {
            this.View.WriteCode(string.Format("{0}.proxy().reset();", this.Id));
        }

        /// <summary>
        /// 对列表进行赋值操作
        /// </summary>
        /// <param name="value"></param>
        public void SetData(DTObject[] data)
        {
            DTObjects list = new DTObjects(data);
            this.View.WriteCode(string.Format("{0}.proxy().setData({1});", this.Id, list.GetCode(false)));
        }

        public void SetData(DTObjects data)
        {
            this.View.WriteCode(string.Format("{0}.proxy().setData({1});", this.Id, data.GetCode(false)));
        }

        #endregion


    }
}
