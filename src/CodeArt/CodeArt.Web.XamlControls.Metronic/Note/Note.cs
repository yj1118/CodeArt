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
        public static DependencyProperty TitleProperty = DependencyProperty.Register<string, Note>("Title", new PropertyMetadata(() => { return string.Empty; }));

        public static DependencyProperty ColorProperty = DependencyProperty.Register<string, Note>("Color", new PropertyMetadata(() => { return string.Empty; }));

        protected override void OnGotClass(ref object baseValue)
        {
            var t = this.Color;
            switch (t)
            {
                case "red": baseValue = "c--stickyNote.c--red"; return;
                case "green": baseValue = "c--stickyNote.c--green"; return;
                case "yellow":
                default: baseValue = "c--stickyNote"; return;
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
        /// red,green,yellow
        /// </summary>
        public string Color
        {
            get
            {
                return GetValue(ColorProperty) as string;
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }

        public Note()
        {
        }

        static Note()
        {
        }
    }
}
