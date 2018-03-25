using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using CodeArt.DTO;
using CodeArt.Util;

namespace CodeArt.Web.WebPages.Xaml.Script
{
    /// <summary>
    /// 脚本视图
    /// </summary>
    [DebuggerDisplay("Type = ScriptView, Input = {_input.GetCode()}")]
    public class ScriptView : IScriptView
    {
        private DTObject _input;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input">脚本视图的输入项</param>
        public ScriptView(DTObject input)
        {
            _input = input;
            InitScript();
            InitSession();
        }

        public ScriptView()
            : this(null)
        {

        }


        private Dictionary<string, ScriptElement> _elementsCache = new Dictionary<string, ScriptElement>();


        public T GetElement<T>(string elementId) where T : ScriptElement
        {
            ScriptElement e = null;

            if (!_elementsCache.TryGetValue(elementId, out e))
            {
                lock (_elementsCache)
                {
                    if (!_elementsCache.TryGetValue(elementId, out e))
                    {
                        DTObject element = GetElementById(elementId);

                        if (element == null)
                        {
                            //没有找到输入项时，采用自由模式
                            element = DTObject.Create("{id,metadata:{}}");
                            element.SetValue("id", elementId);
                            _scriptHeader.AppendFormat("var {0} = $('#{0}');", elementId);
                        }
                        //if (element == null) throw new XamlException("没有找到编号为" + elementId + "的脚本元素");
                        e = ElementFactory.Create<T>(this, element) as T;
                        _elementsCache.Add(elementId, e);
                    }
                }
            }
            return (T)e;
        }

        private DTObject GetElementById(string elementId)
        {
            if (_input == null) return null;
            DTObject element = null;
            //找到元素对应的dto数据
            _input.Each("elements", (ele) =>
            {
                var id = ele.GetValue<string>("id", string.Empty);
                if (string.Equals(id, elementId, StringComparison.OrdinalIgnoreCase))
                {
                    element = ele;
                    return false;
                }
                return true;
            });//{elements:[{id:'xxxx',metadata:xxxx}]}
            return element;
        }


        public HtmlEelementSE GetElement(string elementId)
        {
            return GetElement<HtmlEelementSE>(elementId);
        }


        #region 视图的提交者

        private ScriptElement _sender = null;

        /// <summary>
        /// 获得视图的提交者
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetSender<T>() where T : ScriptElement
        {
            if (_sender == null)
            {
                if (_input == null) throw new XamlException("视图没有提交者");
                var sender = _input.GetObject("sender", null);//{sender:{},elements:[{id:'xxxx',metadata:xxxx}]}
                if (sender == null) throw new XamlException("视图没有提交者");
                _sender = ElementFactory.Create<T>(this, sender);
            }
            return _sender as T;
        }

        /// <summary>
        /// 获得视图的提交者
        /// </summary>
        /// <returns></returns>
        public HtmlEelementSE GetSender()
        {
            return GetSender<HtmlEelementSE>();
        }

        #endregion

        #region 生成脚本

        /// <summary>
        /// 初始化脚本
        /// </summary>
        private void InitScript()
        {
            if (_input != null)
            {
                _input.Each("elements", (ele) =>
                {
                    var id = ele.GetValue<string>("id", string.Empty);
                    //if (string.IsNullOrEmpty(id)) throw new XamlException("脚本元素" + ele.GetCode() + "没有定义编号id");
                    if(!string.IsNullOrEmpty(id)) _scriptHeader.AppendFormat("var {0} = $('#{0}');", id);
                });

                {
                    var sender = _input.GetObject("sender", null);
                    if (sender != null)
                    {
                        var id = sender.GetValue<string>("id", string.Empty);
                        if(!string.IsNullOrEmpty(id)) _scriptHeader.AppendFormat("var {0} = $('#{0}');", id);
                    }
                }
            }
        }

        private StringBuilder _scriptHeader = new StringBuilder();
        private StringBuilder _scriptBody = new StringBuilder();


        /// <summary>
        /// 在输出脚本中声明一个变量
        /// </summary>
        public void Var(string name, DTObject value)
        {
            _scriptBody.AppendFormat("var {0}={1};", name, value.GetCode());
        }

        /// <summary>
        /// 在输出脚本中做if判断
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="trueAction"></param>
        public void If(string condition, Action trueAction)
        {
            If(condition, trueAction, null);
        }

        /// <summary>
        /// 在输出脚本中做if判断
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="trueAction"></param>
        /// <param name="falseAction"></param>
        public void If(string condition, Action trueAction, Action falseAction)
        {
            If(() =>
            {
                _scriptBody.Append(condition);
            }, trueAction, falseAction);
        }

        /// <summary>
        /// 在输出脚本中做if判断
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="trueAction"></param>
        /// <param name="falseAction"></param>
        public void If(Action conditionAction, Action trueAction, Action falseAction)
        {
            _scriptBody.Append("if(");
            conditionAction();
            _scriptBody.Append(")");
            _scriptBody.Append("{");
            trueAction();
            _scriptBody.Append("}");
            if (falseAction != null)
            {
                _scriptBody.Append("else{");
                falseAction();
                _scriptBody.Append("}");
            }
        }

        public void If(Action conditionAction, Action trueAction)
        {
            If(conditionAction, trueAction, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">需要延迟的行为</param>
        /// <param name="millisecond"></param>
        public void SetTimeout(Action action, int millisecond)
        {
            _scriptBody.Append("setTimeout(function () {");
            action();
            _scriptBody.Append(" }, ");
            _scriptBody.AppendFormat("{0});", millisecond);
        }

        /// <summary>
        /// 在输出脚本中做跳转操作
        /// </summary>
        /// <param name="url"></param>
        public void Redirect(string url)
        {
            _scriptBody.AppendFormat("location.href=\"{0}\";", url);
        }

        /// <summary>
        /// 在输出脚本中做消息提示
        /// </summary>
        /// <param name="message"></param>
        public void Alert(string message)
        {
            _scriptBody.AppendFormat("window.alert(\"{0}\");", message);
        }

        /// <summary>
        /// 写入代码
        /// </summary>
        /// <param name="code"></param>
        public void WriteCode(string code)
        {
            _scriptBody.Append(code);
        }

        /// <summary>
        /// 写入一个断点调试
        /// </summary>
        public void WriteDebug()
        {
            _scriptBody.Append("debugger;");
        }

        /// <summary>
        /// 注册一个视图事件处理器，该事件处理器会在视图提交时使用
        /// </summary>
        /// <param name="eventName">视图提交到服务器对应的事件</param>
        internal void RegisterViewEventProcessor(string eventName, ViewEventProcessor proessor)
        {
            _scriptBody.AppendFormat("$$view.registerEventProcessor(\"{0}\",{1});", eventName, proessor.GetCode());
        }

        public void Return(string value)
        {
            _scriptBody.AppendFormat("return \"{0}\";", value);
        }

        public void Return(bool value)
        {
            _scriptBody.AppendFormat("return {0};", value ? "true" : "false");
        }

        /// <summary>
        /// 提交视图，该方法会将该视图重新提交一次
        /// </summary>
        /// <param name="ignoreCustomValidate">是否忽略由脚本视图事件处理器定义的验证</param>
        public void Submit(bool ignoreCustomValidate)
        {
            _scriptBody.Append("$$view.submit({");
            _scriptBody.AppendFormat("ignoreCustomValidate:{0}", ignoreCustomValidate ? "true" : "false");
            _scriptBody.Append("});");
        }

        #endregion


        /// <summary>
        /// 脚本视图输出dto格式的指令
        /// </summary>
        /// <returns></returns>
        public DTObject Output()
        {
            var code = GetCode();
            DTObject output = DTObject.Create();
            output.SetValue("process", code.ToBase64(Encoding.UTF8)); //进行编码，此处编码不是因为DTO的转义而是避免script等特殊标签令客户端执行的时候代码冲突报错，
            return output;
        }

        /// <summary>
        /// 获取脚本代码
        /// </summary>
        /// <returns></returns>
        internal string GetCode()
        {
            var code = new StringBuilder(_scriptHeader.Length + _scriptBody.Length);
            code.Append(_scriptHeader);
            code.Append(_scriptBody);
            return code.ToString();
        }

        /// <summary>
        /// 获取视图数据，以DTO的形式返回
        /// </summary>
        /// <param name="elementIds">需要参与收集数据的元素编号，如果为空那么代表全收集</param>
        /// <returns></returns>
        public DTObject GetData(params string[] elementIds)
        {
            DTObject data = DTObject.Create();
            if (_input == null) return data;
            if (elementIds == null || elementIds.Length == 0)
            {
                //全收集
                _input.Each("elements", (ele) =>
                {
                    var id = ele.GetValue<string>("id", string.Empty);
                    FillData(data, id, ele);
                });
            }
            else
            {
                _input.Each("elements", (ele) =>
                {
                    var id = ele.GetValue<string>("id", string.Empty);
                    if (elementIds.Contains(id)) FillData(data, id, ele);
                });
            }
            return data;
        }

        private void FillData(DTObject data, string elementId, DTObject element)
        {
            if (string.IsNullOrEmpty(elementId)) return;
            var e = ElementFactory.Create<HtmlEelementSE>(this, element);
            if (e.Value == null) return;
            data[elementId] = e.Value;
        }


        #region 视图会话

        /// <summary>
        /// 初始化会话
        /// </summary>
        private void InitSession()
        {
            if (_input == null) return;
            var data = _input.GetOrCreateObject("session");
            _session = new ScriptViewSession(this, data);
        }

        private ScriptViewSession _session;

        public ScriptViewSession Session
        {
            get
            {
                if (_session == null)
                {
                    _session = new ScriptViewSession(this, DTObject.Create());
                }
                return _session;
            }
        }

        #endregion

    }
}
