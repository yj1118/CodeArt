using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.ModuleNest;

namespace CodeArt.Web.XamlControls.Metronic
{
    [ComponentLoaderFactory(typeof(ManualComponentLoader))]
    [ComponentLoader(typeof(InputTextareaLoader))]
    public class Textarea : Input
    {
        public static DependencyProperty RowsProperty { get; private set; }

        static Textarea()
        {
            var rowsMetadata = new PropertyMetadata(() => { return string.Empty; });
            RowsProperty = DependencyProperty.Register<string, Textarea>("Rows", rowsMetadata);
        }

        public string Rows
        {
            get
            {
                return GetValue(RowsProperty) as string;
            }
            set
            {
                SetValue(RowsProperty, value);
            }
        }

        protected override void OnGotType(ref object baseValue)
        {
            baseValue = "textarea";
        }
    }
}