using System.Reflection;
using System.Web;
using System.IO;

using CodeArt.DTO;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages
{
    [SafeAccess]
    public class DTOExtractor : IParameterExtractor
    {
        public virtual DTObject ParseArguments(WebPageContext context)
        {
            HttpRequest request = context.Request;

            string code = null;
            using (StreamReader reader = new StreamReader(request.InputStream))
            {
                code = reader.ReadToEnd();
            }

            if (IsJSON(code)) return DTObject.Create(code);
            else if (IsXML(code)) return DTObject.CreateXml(code);
            else return FormExtractor.Instance.ParseArguments(context);
        }

        private static bool IsJSON(string code)
        {
            code = code.Trim();
            return code.StartsWith("{") && code.EndsWith("}")
                   || code.StartsWith("[") && code.EndsWith("]");
        }

        private static bool IsXML(string code)
        {
            code = code.Trim();
            return code.StartsWith("<xml>") && code.EndsWith("</xml>");
        }


        public static readonly DTOExtractor Instance = new DTOExtractor();

    }
}
