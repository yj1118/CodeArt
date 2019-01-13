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
    public class SelectSE : ScriptElement
    {
        public SelectSE() { }

        private DTObjects _selectedItems;

        public DTObjects SelectedItems
        {
            get
            {
                if (_selectedItems == null)
                {
                    _selectedItems = this.Metadata.GetList("selectedItems");
                }
                return _selectedItems;
            }
        }

        public IEnumerable<object> Values
        {
            get
            {
                return this.SelectedItems.ToArray<object>((t) => t.GetValue("value"));
            }
        }


        public DTObject SelectedItem
        {
            get
            {
                return this.SelectedItems.FirstOrDefault();
            }
        }

        public object Value
        {
            get
            {
                return this.SelectedItem?.GetValue("value", null);
            }
        }


        /// <summary>
        /// 绑定选项，请确保<paramref name="data"/>里有成员为rows的集合作为绑定项
        /// </summary>
        /// <param name="data"></param>
        public void Options(DTObject data)
        {
            using (var temp = StringPool.Borrow())
            {
                var code = temp.Item;

                code.AppendFormat("{0}.proxy().options({1}.rows);", this.Id, data.GetCode(false, false));

                this.View.WriteCode(code.ToString());
            }
        }


        public void SetValue(object value)
        {
            using (var temp = StringPool.Borrow())
            {
                var code = temp.Item;

                code.AppendFormat("{0}.proxy().set({1});", this.Id, JSON.GetCode(value));

                this.View.WriteCode(code.ToString());
            }
        }

        public void Help(string message)
        {
            this.View.WriteCode(string.Format("{0}.proxy().help({1});", this.Id, JSON.GetCode(message)));
        }

    }
}
