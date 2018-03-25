using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.Concurrent;

namespace CodeArt.Web.XamlControls.Metronic
{
    [SafeAccess]
    internal class ModalDialogClassConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            var modal = RenderContext.Current.BelongTemplate.TemplateParent as Modal;
            using (var temp = StringPool.Borrow())
            {
                var sb = temp.Item;
                sb.Append("modal-dialog");
                if (!string.IsNullOrEmpty(modal.Size)) sb.AppendFormat(" modal-{0}", modal.Size);
                if (modal.Center) sb.Append(" modal-dialog-centered");
                return sb.ToString();
            }
        }

        public readonly static ModalDialogClassConverter Instance = new ModalDialogClassConverter();
    }
}
