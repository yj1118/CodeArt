using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using CodeArt.DomainDriven;

namespace MenuSubsystem
{
    public sealed class MenuPath : ValueObject
    {
        public IEnumerable<Menu> Menus
        {
            get;
            private set;
        }

        public MenuPath(IEnumerable<Menu> menus)
        {
            this.Menus = menus;
        }

        public string GetText(string symbol)
        {
            StringBuilder text = new StringBuilder();
            foreach (Menu item in this.Menus)
            {
                text.AppendFormat("{0}{1}", item.Name);
            }
            if (this.Menus.Count() > 0)
                text.Length -= symbol.Length;
            return text.ToString();
        }

        public string GetText()
        {
            return GetText(" > ");
        }

        public static readonly MenuPath Empty = new MenuPath(new List<Menu>());

    }
}
