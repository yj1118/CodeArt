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

            StringBuilder code = new StringBuilder("{");
            foreach (string key in request.Form)
            {
                string value = request.Form[key];
                code.AppendFormat("\"{0}\":\"{1}\",", key, value);
            }

            code.Append("}");

            return DTObject.Create(code.ToString());
        }

        public static readonly FormExtractor Instance = new FormExtractor();

    }
}
