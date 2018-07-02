using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    public class CustomAttribute : DependencyObject, ITemplateCell
    {
        public readonly static DependencyProperty ValueProperty;

        static CustomAttribute()
        {
            var valueMetadata = new PropertyMetadata(() => { return null; });
            ValueProperty = DependencyProperty.Register<object, CustomAttribute>("Value", valueMetadata);
        }

        public string Name
        {
            get;
            private set;
        }

        public object Value
        {
            get
            {
                return GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }
        
        public object Owner
        {
            get;
            private set;
        }

        public string GetPrintValue()
        {
            var value = this.Value;
            return value == null ? string.Empty : value.ToString();
        }

        public CustomAttribute(object owner, string name)
        {
            this.Owner = owner;
            this.Name = name;
        }

        public FrameworkTemplate BelongTemplate
        {
            get
            {
                var u = this.Owner as UIElement;
                return u == null ? null : u.BelongTemplate;
            }
        }
    }
}