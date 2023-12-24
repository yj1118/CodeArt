using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml.Sealed
{
    /// <summary>
    /// 内部元素
    /// 该对象仅提供给密封控件使用
    /// 用于保证密封控件的内部元素是唯一的
    /// </summary>
    public class InternalElements
    {
        public InternalElements()
        {
            this.CollectionPhase = true;
        }

        private List<UIElement> _elements = null;
        private Queue<UIElement> _queue = null;

        /// <summary>
        /// 指示内部元素正在收集阶段
        /// </summary>
        public bool CollectionPhase
        {
            get;
            private set;
        }

        internal void Reset()
        {
            this.CollectionPhase = false;
            if (_elements == null) return;
            _queue = new Queue<UIElement>(_elements.Count);
            foreach (var e in _elements) _queue.Enqueue(e);
        }


        public void Render(PageBrush brush,string xaml)
        {
            if(this.CollectionPhase)
            {
                var e = XamlReader.ReadComponent(xaml) as UIElement;
                if (e == null) return;
                if (_elements == null) _elements = new List<UIElement>();
                _elements.Add(e);
                e.Render(brush);
            }
            else
            {
                //在非收集阶段，需要得到对应的组件，并渲染
                var e = _queue.Dequeue();
                e.Render(brush);
            }
        }

        public void LoadPinned()
        {
            if (_elements == null) return;
            foreach (var e in _elements) e.LoadPinned();
        }

        public DependencyObject GetChild(string childName)
        {
            if (_elements == null) return null;
            var childs = this;
            foreach (var e in _elements)
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

    }
}
