using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    [ContentProperty("Setters")]
    public class XamlStyle : DependencyObject, IStyle
    {
        public static DependencyProperty BasedOnProperty { get; private set; }

        public static DependencyProperty SettersProperty { get; private set; }

        public static DependencyProperty TargetTypeProperty { get; private set; }

        static XamlStyle()
        {
            var basedOnMetadata = new PropertyMetadata(() => { return null; });
            BasedOnProperty = DependencyProperty.Register<string, XamlStyle>("BasedOn", basedOnMetadata);

            var settersMetadata = new PropertyMetadata(() => { return new SetterCollection(); });
            SettersProperty = DependencyProperty.Register<SetterCollection, XamlStyle>("Setters", settersMetadata);

            var targetTypeMetadata = new PropertyMetadata(() => { return null; });
            TargetTypeProperty = DependencyProperty.Register<Type, XamlStyle>("TargetType", targetTypeMetadata);
        }

        public string BasedOn
        {
            get
            {
                return (string)GetValue(BasedOnProperty);
            }
            set
            {
                SetValue(BasedOnProperty, value);
            }
        }

        public SetterCollection Setters
        {
            get
            {
                return GetValue(SettersProperty) as SetterCollection;
            }
            set
            {
                SetValue(SettersProperty, value);
            }
        }

        [TypeConverter(typeof(StyleTargetTypeConvert))]
        public Type TargetType
        {
            get
            {
                return GetValue(TargetTypeProperty) as Type;
            }
            set
            {
                SetValue(TargetTypeProperty, value);
            }
        }

        public void Apply(DependencyObject target)
        {
            //先应用基类的
            var basedOn = this.BasedOn;
            if (basedOn != null)
            {
               var style = Application.Current.FindResource(basedOn) as XamlStyle;
                style.Apply(target);
            }

            //再应用当前的
            var setters = this.Setters;
            foreach (var item in setters)
            {
                var setter = item as Setter;
                target.SetValue(setter.Property, setter.GetActualValue(Setter.ValueProperty));
            }
        }

        public object GetSetterValue(string propertyName)
        {
            var setters = this.Setters;
            foreach (var item in setters)
            {
                var setter = item as Setter;
                if (setter.Property.Name == propertyName)
                    return setter.GetActualValue(Setter.ValueProperty);
            }
            return null;
        }

    }
}
