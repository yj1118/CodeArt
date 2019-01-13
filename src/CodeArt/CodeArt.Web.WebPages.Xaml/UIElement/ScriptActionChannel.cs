using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Script;

namespace CodeArt.Web.WebPages.Xaml
{
    internal class ScriptActionChannel
    {
        private ConcurrentDictionary<string, UIElement> _elements = new ConcurrentDictionary<string, UIElement>();

        public ScriptActionChannel()
        {

        }
    }
}
