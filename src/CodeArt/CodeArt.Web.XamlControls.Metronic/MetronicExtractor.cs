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
        public override DTObject ParseArguments(WebPageContext context)
        {
            var arg = base.ParseArguments(context);
            if(DatatableExtractor.Exist(arg))
            {
                arg = DatatableExtractor.Transform(arg);
            }
            return arg;
        }

        public new static readonly MetronicExtractor Instance = new MetronicExtractor();

    }
}
