using System.Reflection;
using System.Web;
using System.IO;

using CodeArt.DTO;
using CodeArt.Web.WebPages;
using CodeArt.Concurrent;

namespace CodeArt.Web.XamlControls.Metronic
{
    [SafeAccess]
    public class MetronicExtractor : DTOExtractor
    {
        public override object[] ParseArguments(WebPageContext context)
        {
            var args = base.ParseArguments(context);
            var arg = args[0] as DTObject;
            if(DatatableExtractor.Exist(arg))
            {
                arg = DatatableExtractor.Transform(arg);
                args[0] = arg;
            }
            return args;
        }

        public new static readonly MetronicExtractor Instance = new MetronicExtractor();

    }
}
