using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;


namespace CodeArt.Web.XamlControls.Metronic
{
    /// <summary>
    /// form的内部区域，带标题
    /// </summary>
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Form.FormSectionTemplate.html,CodeArt.Web.XamlControls.Metronic")]
    public class FormSection : ContentControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register<string, FormSection>("Title", () => { return string.Empty; });

        public string Title
        {
            get
            {
                return GetValue(TitleProperty) as string;
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }
    }
}
