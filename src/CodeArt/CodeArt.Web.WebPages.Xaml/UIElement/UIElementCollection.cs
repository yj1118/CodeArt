using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.WebPages.Xaml
{
    public class UIElementCollection : DependencyCollection, IUIElement
    {
        public void Render(PageBrush brush)
        {
            foreach(var e in this)
            {
                var ui = e as IUIElement;
                if (e != null) ui.Render(brush);
            }
        }

        public DependencyObject GetChild(string childName)
        {
            var childs = this;
            foreach (var e in childs)
            {
                var ui = e as UIElement;
                if (ui != null)
                {
                    if (string.Equals(ui.Name, childName, StringComparison.OrdinalIgnoreCase)) return ui;
                    var child = ui.GetChild(childName);
                    if (child != null) return child;
                }
            }
            return null;
        }

        public IEnumerable<UIElement> GetActionElement(string actionName)
        {
            List<UIElement> elems = null;
            var childs = this;
            foreach (var e in childs)
            {
                var ui = e as UIElement;
                if (ui != null)
                {
                    var t = ui.GetActionElement(actionName);
                    if (t != null)
                    {
                        if (elems == null) elems = new List<UIElement>();
                        elems.AddRange(t);
                    }
                }
            }
            return elems;
        }

    }
}
