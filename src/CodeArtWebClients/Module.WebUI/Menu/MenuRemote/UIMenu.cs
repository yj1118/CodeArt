using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Module.WebUI
{
    public sealed class UIMenu
    {
        private IEnumerable<UIMenuItem> _items;
        public IEnumerable<UIMenuItem> Items
        {
            get
            {
                return _items;
            }
        }

        public UIMenu(IEnumerable<UIMenuItem> items)
        {
            _items = items;
        }

        public string ToJSON()
        {
            StringBuilder code = new StringBuilder("[");
            if (_items.Count() > 0)
            {
                foreach (var item in _items)
                {
                    code.AppendFormat("{0},", item.ToJSON());
                }
                code.Length--;
            }
            code.Append("]");
            return code.ToString();
        }
    }
}
