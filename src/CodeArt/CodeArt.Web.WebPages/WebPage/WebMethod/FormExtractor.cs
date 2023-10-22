using System.Reflection;
using System.Web;
using System.IO;
using System.Text;

using CodeArt.DTO;

namespace CodeArt.Web.WebPages
{
    internal sealed class FormExtractor : IParameterExtractor
    {
        public DTObject ParseArguments(WebPageContext context)
        {
            HttpRequest request = context.Request;

            DTObject data = DTObject.Create();
            foreach (string key in request.Form)
            {
                string value = request.Form[key];
                data[key] = value;
            }

            return data;
        }

        public static readonly FormExtractor Instance = new FormExtractor();

    }
}
