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

        public string Code
        {
            get;
            private set;
        }

        public SqlCondition(string code)
        {
            _likes = new List<SqlLike>();
            _ins = new List<SqlIn>();
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
            return code;
        }

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

                    var exp = g.Value.Trim();
                    bool before = exp.StartsWith("%");
                    bool after = exp.EndsWith("%");
                    exp = exp.Trim('%');
                    var para = exp.Substring(1);
                    exp = string.Format(" {0}", exp);//补充一个空格

                    SqlLike like = new SqlLike(para, before, after);
                    _likes.Add(like);

                    code = code.Insert(g.Index + offset, exp);
                    code = code.Remove(g.Index + offset + exp.Length, length);

                    offset += length - exp.Length; //记录偏移量
                }
            }
            return code;
        }

        private string ParseIn(string code)
        {
            using (var temp = _inRegex.Borrow())
            {
                var regex = temp.Item;
                var ms = regex.Matches(code);
                int offset = 0;
                foreach (Match m in ms)
                {
                    var g = m.Groups[1];
                    int length = g.Value.Length;

                    var exp = g.Value.Trim();
                    var para = exp.Substring(1);
                    exp = string.Format("(@{0})", para);

                    SqlIn sin = new SqlIn(para, exp);
                    _ins.Add(sin);

                    code = code.Insert(g.Index + offset, exp);
                    code = code.Remove(g.Index + offset + exp.Length, length);

                    offset += length - exp.Length; //记录偏移量
                }
            }
            return code;
        }


        public override string ToString()
        {
            return this.Code;
        }


        internal string Process(string commandText, DynamicData param)
        {
            if (this.IsEmpty()) return commandText;

            foreach(var like in _likes)
            {
                var name = like.ParamName;
                var value = param.Get(name) as string;
                if (value == null) throw new DataAccessException(string.Format(Strings.QueryParamTypeError, name));

                if (like.After && like.Before) param.Set(name, string.Format("%{0}%", value));
                else if(like.After) param.Set(name, string.Format("{0}%", value));
                else if(like.Before) param.Set(name, string.Format("%{0}", value));
            }

            foreach(var sin in _ins)
            {
                var name = sin.ParamName;
                var values = param.Get(name) as IEnumerable;
                if (values == null) throw new DataAccessException(string.Format(Strings.QueryParamTypeError, name));
                param.Remove(name);

                using (var temp = StringPool.Borrow())
                {
                    var code = temp.Item;
                    int index = 0;
                    code.Append("(");
                    foreach (var value in values)
                    {
                        var valueName = string.Format("{0}{1}", name, index);
                        param.Add(valueName, value);
                        code.AppendFormat("@{0},", valueName);
                        index++;
                    }
                    if (code.Length > 1) code.Length--;
                    else code.Append("''"); //避免空数据时报错
                    code.Append(")");

                    commandText = commandText.Replace(sin.Placeholder, code.ToString());
                }
            }

            return commandText;

        }



        private static RegexPool _likeRegex = new RegexPool(@"[ ]+?like([ %]+?@[%\d\w0-9]+)", RegexOptions.IgnoreCase);

        private static RegexPool _inRegex = new RegexPool(@"[ ]+?in[ ]+?(@[\d\w0-9]+)", RegexOptions.IgnoreCase);




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