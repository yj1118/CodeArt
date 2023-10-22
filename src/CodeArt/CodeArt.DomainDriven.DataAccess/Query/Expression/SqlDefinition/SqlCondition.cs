using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections;
using System.Text;

using CodeArt.DomainDriven;
using CodeArt.Util;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven.DataAccess
{
    [DebuggerDisplay("{Code}")]
    public class SqlCondition
    {
        private List<SqlLike> _likes;

        public IEnumerable<SqlLike> Likes
        {
            get
            {
                return _likes;
            }
        }


        private List<SqlIn> _ins;

        public IEnumerable<SqlIn> Ins
        {
            get
            {
                return _ins;
            }
        }

        private List<SqlAny> _anys;

        public IEnumerable<SqlAny> Anys
        {
            get
            {
                return _anys;
            }
        }

        public string Code
        {
            get;
            private set;
        }

        /// <summary>
        /// 探测代码，我们增加了很多自定义语法，这些语法sql引擎不能识别，
        /// 在探测查询列时，我们需要用sql引擎可以识别的语法，所以需要探测代码，该代码是sql可以识别的
        /// </summary>
        public string ProbeCode
        {
            get;
            private set;
        }


        public SqlCondition(string code)
        {
            _likes = new List<SqlLike>();
            _ins = new List<SqlIn>();
            _anys = new List<SqlAny>();
            this.Code = Parse(code);
        }

        public bool IsEmpty()
        {
            return this.Code.Length == 0;
        }


        private string Parse(string code)
        {
            if (string.IsNullOrEmpty(code)) return code;
            code = ParseLike(code);
            code = ParseIn(code);
            code = ParseAny(code);
            return code;
        }

        /// <summary>
        /// 解析like写法 like %@name% 转为可以执行的语句
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private string ParseLike(string code)
        {
            using (var temp = _likeRegex.Borrow())
            {
                var regex = temp.Item;
                var ms = regex.Matches(code);
                int offset = 0;
                foreach (Match m in ms)
                {
                    var g = m.Groups[1];
                    int length = g.Value.Length;

                    var newExp = g.Value.Trim();
                    bool before = newExp.StartsWith("%");
                    bool after = newExp.EndsWith("%");
                    newExp = newExp.Trim('%');
                    var para = newExp.Substring(1);
                    newExp = string.Format(" {0}", newExp);//补充一个空格

                    SqlLike like = new SqlLike(para, before, after);
                    _likes.Add(like);

                    code = code.Insert(g.Index + offset, newExp);
                    code = code.Remove(g.Index + offset + newExp.Length, length);

                    offset += newExp.Length - length; //记录偏移量
                }
            }
            return code;
        }

        /// <summary>
        /// 解析in写法 id in @ids 转为可以执行的语句
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private string ParseIn(string code)
        {
            using (var temp = _inRegex.Borrow())
            {
                var regex = temp.Item;
                var ms = regex.Matches(code);
                int offset = 0;
                foreach (Match m in ms)
                {
                    var g = m.Groups[0];
                    int length = g.Value.Length;

                    var field = m.Groups[1].Value;

                    var newExp = m.Groups[2].Value.Trim();
                    var para = newExp.Substring(1);
                    newExp = string.Format("{0} in (@{0})",field, para);

                    SqlIn sin = new SqlIn(field, para, newExp);
                    _ins.Add(sin);

                    code = code.Insert(g.Index + offset, newExp);
                    code = code.Remove(g.Index + offset + newExp.Length, length);

                    offset += newExp.Length - length; //记录偏移量
                }
            }
            return code;
        }

        /// <summary>
        /// 解析任意条件的写法 @name{name like @name} 转为可以执行的语句
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private string ParseAny(string code)
        {
            var probeCode = code;
            using (var temp = _anyRegex.Borrow())
            {
                var regex = temp.Item;
                var ms = regex.Matches(code);
                foreach (Match m in ms)
                {
                    var placeholder = m.Groups[0].Value;
                    var paraName = m.Groups[1].Value;
                    var content = m.Groups[2].Value;
                    //转义
                    content = content.Replace("&lt;","<").Replace("&gt;", ">");

                    SqlAny any = new SqlAny(paraName, placeholder, content);
                    _anys.Add(any);

                    probeCode = probeCode.Replace(placeholder, content);
                }
            }

            this.ProbeCode = probeCode;

            return code;
        }

        public override string ToString()
        {
            return this.Code;
        }


        internal string Process(string commandText, DynamicData param)
        {
            if (this.IsEmpty()) return commandText;

            //先处理any
            foreach(var any in _anys)
            {
                if(param.ContainsKey(any.ParamName))
                    commandText = commandText.Replace(any.Placeholder, any.Content);
                else
                    commandText = commandText.Replace(any.Placeholder, "0=0"); //没有参数表示永远为真，这里不能替换为空文本，因为会出现where 没有条件的BUG，所以为 where 0=0
            }


            foreach(var like in _likes)
            {
                var name = like.ParamName;
                var value = param.Get(name) as string;
                if (value == null) continue; //因为有any语法，所以没有传递参数也正常

                if (like.After && like.Before) param.Set(name, string.Format("%{0}%", value));
                else if(like.After) param.Set(name, string.Format("{0}%", value));
                else if(like.Before) param.Set(name, string.Format("%{0}", value));
            }

            foreach(var sin in _ins)
            {
                var name = sin.ParamName;
                var values = param.Get(name) as IEnumerable;
                param.Remove(name);

                if(values != null && values.Exists())
                {
                    using (var temp = StringPool.Borrow())
                    {
                        var code = temp.Item;
                        int index = 0;
                        code.AppendFormat("{0} in (", sin.Field);
                        foreach (var value in values)
                        {
                            var valueName = string.Format("{0}{1}", name, index);
                            param.Add(valueName, value);
                            code.AppendFormat("@{0},", valueName);
                            index++;
                        }
                        if (code.Length > 1) code.Length--;
                        code.Append(")");

                        commandText = commandText.Replace(sin.Placeholder, code.ToString());
                    }
                }
                else
                {
                    commandText = commandText.Replace(sin.Placeholder, "0=1"); //在in语句中，如果 id in ()，没有任何匹配的数值条件，那么就是无匹配结果，所以0=1
                }
            }

            return commandText;

        }



        private static RegexPool _likeRegex = new RegexPool(@"[ ]+?like([ %]+?@[%\d\w0-9]+)", RegexOptions.IgnoreCase);

        private static RegexPool _inRegex = new RegexPool(@"([\d\w0-9]+?)[ ]+?in[ ]+?(@[\d\w0-9]+)", RegexOptions.IgnoreCase);

        private static RegexPool _anyRegex = new RegexPool(@"@([^\< ]+)\<([^\>]+)\>", RegexOptions.IgnoreCase);


        //#region 查询相关


        ///// <summary>
        ///// 将数据以通配符的形式加入，这常用于查询中的like语句
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="value"></param>
        //public void Like(string name, string value)
        //{
        //    _data.Add(name, string.Format("%{0}%", value));
        //}

        ///// <summary>
        ///// 将数据以前置通配符的形式加入，这常用于查询中的like语句
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="value"></param>
        //public void LikeStart(string name, string value)
        //{
        //    _data.Add(name, string.Format("%{0}", value));
        //}

        ///// <summary>
        ///// 将数据以后置通配符的形式加入，这常用于查询中的like语句
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="value"></param>
        //public void LikeEnd(string name, string value)
        //{
        //    _data.Add(name, string.Format("{0}%", value));
        //}


        //#endregion



        public static readonly SqlCondition Empty = new SqlCondition(string.Empty);


    }
}