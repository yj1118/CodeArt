using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    [ComponentLoader(typeof(SetterLoader))]
    public class Setter : DependencyObject, ISetter
    {
        public static DependencyProperty PropertyProperty { get; private set; }

        public static DependencyProperty ValueProperty { get; private set; }

        static Setter()
        {
            var propertyMetadata = new PropertyMetadata(() => { return null; });
            PropertyProperty = DependencyProperty.Register<DependencyProperty, Setter>("Property", propertyMetadata);

            var valueMetadata = new PropertyMetadata(() => { return null; });
            ValueProperty = DependencyProperty.Register<object, Setter>("Value", valueMetadata);
        }


        public DependencyProperty Property
        {
            get
            {
                return GetValue(PropertyProperty) as DependencyProperty;
            }
            set
            {
                SetValue(PropertyProperty, value);
            }
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
    }
}
