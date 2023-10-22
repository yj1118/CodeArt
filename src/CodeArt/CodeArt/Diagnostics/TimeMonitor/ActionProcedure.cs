using System;
using System.Collections.Generic;


namespace CodeArt.Diagnostics
{
    public sealed class ActionProcedure
    {
        private List<ActionItem> _items = new List<ActionItem>();
        internal List<ActionItem> Items
        {
            get { return _items; }
        }

        public ActionProcedure()
        {
            
        }

        public void AddAction(string name,Action action)
        {
            this.Items.Add(new ActionItem { Name = name, Action = action });
        }

        public void AddAction(Action action)
        {
            this.AddAction(string.Empty, action);
        }

        public void Clear()
        {
            if (_items != null)
                _items.Clear();
        }

    }
}
