using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Script;

namespace CodeArt.Web.XamlControls.Metronic
{
    /// <summary>
    /// 
    /// </summary>
    public class DropdownSE : ScriptElement
    {
        public DropdownSE() { }

        /// <summary>
        /// 该方法不会产生发射代码
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValue<T>(T defaultValue)
        {
            return this.Metadata.GetValue<T>("value", defaultValue);
        }


        /// <summary>
        /// 该属性用于多级下拉组件
        /// </summary>
        public T[] GetValues<T>()
        {
            var list = this.Metadata.GetList("value", false);
            return list == null ? Array.Empty<T>() : list.ToArray<T>();
        }

        /// <summary>
        /// 获取当前组件有效的父亲编号，该方法常用于加载数据时使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetParentValue<T>(T defaultValue)
        {
            return this.Metadata.GetValue<T>("parentValue", defaultValue);
        }


        #region 发射命令

        /// <summary>
        /// 设置值
        /// </summary>
        public void SetValue(object value)
        {
            this.View.WriteCode(string.Format("{0}.proxy().set({1});", this.Id, JSON.GetCode(value)));
        }

        /// <summary>
        /// 绑定下拉数据，该方法仅在单级下拉框中有效
        /// </summary>
        /// <param name="data"></param>
        public void Bind(DTObject data)
        {
            StringBuilder code = new StringBuilder();

            code.AppendFormat("var items{0}={1}.items;", this.Id, data.GetCode());
            code.AppendFormat("{0}.proxy().options(items{0});", this.Id);

            this.View.WriteCode(code.ToString());
        }

        public void SelectItem(int index)
        {
            this.View.WriteCode(string.Format("{0}.proxy().selectItem({1});", this.Id, index));
        }

        /// <summary>
        /// 加载下拉数据，该方法仅在多级下拉框中有效
        /// </summary>
        /// <param name="para">参数</param>
        public void Load()
        {
            this.View.WriteCode(string.Format("{0}.proxy().init();", this.Id));
        }


        /// <summary>
        /// 加载下拉数据，该方法仅在多级下拉框中有效
        /// </summary>
        /// <param name="para">参数</param>
        public void Load(Action callBack)
        {
            this.View.WriteCode(string.Format("{0}.proxy().init(function()", this.Id));
            this.View.WriteCode("{");
            callBack();
            this.View.WriteCode("});");
        }

        public void Browse(bool flag)
        {
            string isBrowse = flag ? "true" : "false";
            this.View.WriteCode(string.Format("{0}.proxy().browse({1});", this.Id, isBrowse));
        }

        #endregion
    }
}
