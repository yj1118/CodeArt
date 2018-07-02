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
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Note.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class Note : ContentControl
    {
        public static DependencyProperty TitleProperty { get; private set; }

        public static DependencyProperty TypeProperty { get; private set; }

        static Note()
        {
            var titleMetadata = new PropertyMetadata(() => { return string.Empty; });
            TitleProperty = DependencyProperty.Register<string, Note>("Title", titleMetadata);

            var typeMetadata = new PropertyMetadata(() => { return string.Empty; });
            TypeProperty = DependencyProperty.Register<string, Note>("Type", typeMetadata);
        }

        protected override void OnGotClass(ref object baseValue)
        {
            var t = this.Type;
            switch (t)
            {
                case "success": baseValue = "note note-success"; return;
                case "warning": baseValue = "note note-warning"; return;
                case "danger": baseValue = "note note-danger"; return;
                default: baseValue = "note note-info"; return;
            }
        }

        /// <summary>
        /// 标题
        /// </summary>
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

        /// <summary>
        /// success、warning、danger
        /// </summary>
        public string Type
        {
            get
            {
                return GetValue(TypeProperty) as string;
            }
            set
            {
                SetValue(TypeProperty, value);
            }
        }

        public Note()
        {
        }
    }
}
