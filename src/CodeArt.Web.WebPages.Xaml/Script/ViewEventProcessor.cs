using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.WebPages.Xaml.Script
{
    /// <summary>
    /// 脚本视图事件处理器
    /// </summary>
    public sealed class ViewEventProcessor
    {
        private Dictionary<ViewEvent, ScriptView> _data = null;

        public void On(ViewEvent evt, Action<ScriptView> action)
        {
            if (_data == null) _data = new Dictionary<ViewEvent, ScriptView>();
            ScriptView view = null;
            if(!_data.TryGetValue(evt,out view))
            {
                view = new ScriptView();
                _data.Add(evt, view);
            }
            action(view);
        }


        public string GetCode()
        {
            StringBuilder code = new StringBuilder();
            code.Append("new function(){");
            if(_data !=null)
            {
                foreach(var p in _data)
                {
                    var evt = p.Key;
                    var view = p.Value;

                    code.AppendFormat(" this.{0}=function()", GetEventName(evt));
                    code.Append("{");
                    code.Append(view.GetCode());
                    code.Append("};");
                }
            }
            code.Append("}");

            return code.ToString();
        }

        private static string GetEventName(ViewEvent evt)
        {
            switch (evt)
            {
                case ViewEvent.Validate: return "validate";
            }
            throw new XamlException("未知的错误 ViewEventProcessor.GetEventName");
        }

    }
}
