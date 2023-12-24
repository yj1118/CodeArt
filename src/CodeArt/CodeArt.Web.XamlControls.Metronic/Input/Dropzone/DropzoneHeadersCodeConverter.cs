using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Concurrent;
using CodeArt.DTO;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class DropzoneHeadersCodeConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            var d = RenderContext.Current.BelongTemplate.TemplateParent as Dropzone;
            var headrs = d.Headers;
            using (var temp = StringPool.Borrow())
            {
                var code = temp.Item;
                code.Append("{");
                foreach(DropzoneHeader header in headrs)
                {
                    code.AppendFormat("{0}:'{1}',", header.Key, JSON.GetCode(header.Value).Trim('"'));
                }
                code.Append("}");
                return code.ToString();
            }
        }


        public readonly static DropzoneHeadersCodeConverter Instance = new DropzoneHeadersCodeConverter();
    }
}