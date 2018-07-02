using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

using CodeArt.Web.WebPages.Xaml.Script;

namespace CodeArt.Web.XamlControls.Metronic
{
    /// <summary>
    /// 
    /// </summary>
    public class AdminPageView : IScriptView
    {
        private ScriptView _view;

        public AdminPageView(ScriptView view)
        {
            _view = view;
        }

        public DTObject Output()
        {
            return _view.Output();
        }

        /// <summary>
        /// 追加一个站点路径
        /// </summary>
        /// <param name="text"></param>
        /// <param name="url"></param>
        public void AddSitePath(string text, string url)
        {
            var code = string.Join(string.Empty, "$$(\"#sitePath\").push({ text: \"", text, "\", url: \"", url, "\" });");
            _view.WriteCode(code);
        }

        /// <summary>
        /// 追加一个站点路径
        /// </summary>
        /// <param name="text"></param>
        public void AddSitePath(string text)
        {
            var code = string.Join(string.Empty, "$$(\"#sitePath\").push({ text: \"" , text , "\", click: function (e) { return false; } });");
            _view.WriteCode(code);
        }

    }
}
