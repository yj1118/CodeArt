using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using CodeArt.DomainDriven;
using CodeArt.Util;


namespace CodeArt.DomainDriven.DataAccess
{
    public class SqlDefinition
    {
        /// <summary>
        /// top限定语句
        /// </summary>
        public string Top
        {
            get;
            private set;
        }

        /// <summary>
        /// select语句指定查询的字段名称
        /// </summary>
        public string SelectFields
        {
            get;
            private set;
        }

        /// <summary>
        /// 在查询中需要输出的字段名集合
        /// </summary>
        public string OutputFileds
        {
            get;
            private set;
        }

        public string GetFieldsSql()
        {
            if(this.OutputFileds == null)
            {
                if (string.IsNullOrEmpty(this.SelectFields)) this.OutputFileds = "*";
                else
                {
                    //where涉及到的字段内置到GetObjectSql中，所以不必考虑
                    List<string> temp = new List<string>();
                    temp.AddRange(this.Columns.Select);
                    temp.AddRange(this.Columns.Order);
                    this.OutputFileds = string.Join(",", temp.Distinct(StringComparer.OrdinalIgnoreCase));
                }
            }
            return this.OutputFileds;
        }


        /// <summary>
        /// 是否指定了加载哪些字段
        /// </summary>
        public bool IsSpecifiedField
        {
            get
            {
                return !this.Columns.IsAll;
            }
        }


        /// <summary>
        /// 查询条件
        /// </summary>
        public SqlCondition Condition
        {
            get;
            private set;
        }


        /// <summary>
        /// 排序
        /// </summary>
        public string Order
        {
            get;
            private set;
        }

        internal SqlColumns Columns
        {
            get;
            private set;
        }

        public string Key
        {
            get;
            private set;
        }

        /// <summary>
        /// 是否为自定义sql，这表示由程序员自己翻译执行的sql语句
        /// </summary>
        public bool IsCustom
        {
            get
            {
                return !string.IsNullOrEmpty(this.Key);
            }
        }

        public bool IsNativeSql
        {
            get;
            private set;
        }

        public string NativeSql
        {
            get;
            private set;
        }

        private SqlDefinition()
        {
            this.Top = string.Empty;
            this.SelectFields = string.Empty;
            this.Condition = SqlCondition.Empty;
            this.Order = string.Empty;
            this.Columns = SqlColumns.Empty;
            this.Key = string.Empty;
        }

        private SqlDefinition(SqlColumns columns)
            : this()
        {
            this.Columns = columns;
        }

        /// <summary>
        /// 对象链是否包括在该表达式中,对象链的格式是 a_b_c
        /// </summary>
        /// <param name="ObjectChain"></param>
        /// <returns></returns>
        public bool ContainsChain(string objectChain)
        {
            if (this.IsEmpty) return false;

            var objectChainOffset = _getObjectChainOffset(objectChain);
            return ContainsChain(this.Columns.Select, objectChainOffset)
                    || ContainsChain(this.Columns.Where, objectChainOffset)
                    || ContainsChain(this.Columns.Order, objectChainOffset);
        }

        private static Func<string, string> _getObjectChainOffset = LazyIndexer.Init<string, string>((objectChain) =>
        {
            return string.Format("{0}_", objectChain);
        });


        private static bool ContainsChain(IEnumerable<string> fields,string target)
        {
            foreach(var field in fields)
            {
                if (field.IndexOf(target, StringComparison.OrdinalIgnoreCase) > -1) return true;
            }
            return false;
        }



        /// <summary>
        /// 判断表<paramref name="table"/>的字段是否出现在当前sql定义中，（除了ID以外的字段）
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public bool ContainsExceptId(DataTable table)
        {
            if (this.IsEmpty) return false;

            var fields = table.Fields;
            foreach(var field in fields)
            {
                if (field.Name == EntityObject.IdPropertyName) continue;
                if (this.Columns.Contains(field.Name)) return true;
            }
            return false;
        }


        public bool ContainsField(string fieldName)
        {
            return this.Columns.Contains(fieldName);
        }

        /// <summary>
        /// 确实手工指定了某个字段,与ContainsField不同,当 select * 时，Contains返回的是true
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public bool SpecifiedField(string fieldName)
        {
            return this.Columns.Specified(fieldName);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsEmpty
        {
            get;
            private set;
        }

        private static bool _IsEmpty(SqlDefinition def)
        {
            return def.Top.Length == 0
                    && def.SelectFields.Length == 0
                    && def.Condition.IsEmpty()
                    && def.Order.Length == 0;
        }


        public static SqlDefinition Create(string expression,bool isEnabledMultiTenancy)
        {
            if (string.IsNullOrEmpty(expression)) return Empty;
            return _create(expression)(isEnabledMultiTenancy);
        }

        private static Func<string, Func<bool, SqlDefinition>> _create = LazyIndexer.Init<string, Func<bool, SqlDefinition>>((expression) =>
        {
            return LazyIndexer.Init<bool, SqlDefinition>((isEnabledMultiTenancy) =>
            {
                SqlDefinition define = new SqlDefinition();
                const string sqlTip = "[sql]";
                if (expression.StartsWith("[sql]"))
                {
                    define.IsNativeSql = true;
                    define.NativeSql = expression.Substring(sqlTip.Length);
                }
                else
                {
                    var subs = CollectSubs(expression);

                    foreach (var sub in subs)
                    {
                        var exp = Pretreatment(sub);
                        if (string.IsNullOrEmpty(exp)) continue;

                        if (IsTop(exp)) define.Top = exp;
                        else if (IsOrder(exp)) define.Order = exp;
                        else if (IsSelect(exp)) define.SelectFields = GetFields(exp);
                        else if (IsKey(exp)) define.Key = GetKey(exp);
                        else
                        {
                            if (isEnabledMultiTenancy)
                            {
                                if (string.IsNullOrEmpty(exp))
                                    exp = string.Format("@{0}<{0}=@{0}>", GeneratedField.TenantIdName);
                                else
                                    exp = string.Format("{0} and @{1}<{1}=@{1}>", exp, GeneratedField.TenantIdName);
                            }

                            define.Condition = new SqlCondition(exp); //默认为条件
                        }
                    }
                    define.IsEmpty = _IsEmpty(define); //缓存结果，运行时不必再运算
                    define.Columns = GetColumns(define);
                }
                return define;
            });
        });


        private static SqlColumns GetColumns(SqlDefinition define)
        {
            var mockSql = string.Format("select {0} from tempTable {1} {2}",
                                                                            string.IsNullOrEmpty(define.SelectFields) ? "*" : define.SelectFields,
                                                                            define.Condition.IsEmpty() ? string.Empty : string.Format("where {0}", define.Condition.ProbeCode),
                                                                            define.Order);
            return SqlParser.Parse(mockSql);
        }

        /// <summary>
        /// 预处理
        /// </summary>
        /// <returns></returns>
        private static string Pretreatment(string expression)
        {
            if (expression.StartsWith("["))
            {
                expression = expression.Substring(1, expression.Length - 2);
            }

            expression = expression.Trim();
            return expression.Replace(".", "_"); //将属性路径中的 . 转换成字段分隔 _
        }


        private static bool IsTop(string expression)
        {
            return expression.StartsWith("top ", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsKey(string expression)
        {
            return expression.StartsWith("key ", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 使用select 指定需要查询的字段名称
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static bool IsSelect(string expression)
        {
            return expression.StartsWith("select ", StringComparison.OrdinalIgnoreCase);
        }

        private static int _selectLength = "select ".Length;

        private static string GetFields(string expression)
        {
            int startIndex = _selectLength;
            return expression.Substring(startIndex).Trim();
        }

        private static int _keyLength = "key ".Length;
        private static string GetKey(string expression)
        {
            int startIndex = _keyLength;
            return expression.Substring(startIndex).Trim();
        }


        private static bool IsOrder(string expression)
        {
            return expression.StartsWith("order by", StringComparison.OrdinalIgnoreCase);
        }

        #region 子表达式

        /// <summary>
        /// 收集子表达式
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static List<string> CollectSubs(string expression)
        {
            List<string> subs = new List<string>();
            int pointer = 0;
            while (pointer < expression.Length)
            {
                var sub = FindSub(expression, pointer);
                subs.Add(sub);
                pointer += sub.Length;
            }
            return subs;
        }

        /// <summary>
        /// 找到子表达式
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        private static string FindSub(string expression, int startIndex)
        {
            if (expression[startIndex] != '[')
            {
                var endIndex = expression.IndexOf('[', startIndex);
                if (endIndex > 0)
                {
                    //格式：aaa[bbbb]
                    return expression.Substring(startIndex, endIndex - startIndex);
                }
                else
                {
                    //格式：aaa
                    return expression.Substring(startIndex);
                }
            }
            else
            {
                var endIndex = expression.IndexOf(']', startIndex);
                if (endIndex > 0)
                {
                    //格式：[aaa][bbbb]
                    return expression.Substring(startIndex, endIndex - startIndex + 1);
                }
                else
                {
                    //格式：[aaa
                    throw new DataPortalException(string.Format(Strings.QueryExpressionMalformed, expression));
                }
            }
        }

        #endregion


        #region 处理命令文本

        internal string Process(string commandText, DynamicData param)
        {
            if (this.IsNativeSql) return this.NativeSql;
            if (string.IsNullOrEmpty(commandText) || this.IsEmpty) return commandText;
            return this.Condition.Process(commandText, param);
        }


        #endregion




        public static readonly SqlDefinition Empty = new SqlDefinition();

        /// <summary>
        /// 除根之外都加载
        /// </summary>
        public static readonly SqlDefinition All = new SqlDefinition(SqlColumns.All);

    }
}