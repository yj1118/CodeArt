using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.IO;

using CodeArt.Runtime;
using CodeArt.Util;

namespace CodeArt.DomainDriven.DataAccess
{
    internal static class SqlParser
    {
        /// <summary>
        /// 分析sql语句，获得参与查询的列信息
        /// </summary>
        /// <param name="sqlSelect"></param>
        public static SqlColumns Parse(string sql)
        {
            return _getColumns(sql);
        }

        private static Func<string, SqlColumns> _getColumns = LazyIndexer.Init<string, SqlColumns>((sql)=>
        {
            TSql120Parser parser = new TSql120Parser(true);
            using (TextReader reader = new StringReader(sql))
            {
                IList<ParseError> errors = null;
                var fragment = parser.Parse(reader, out errors);
                CheckUpError(sql, errors);

                try
                {
                    var query = ((fragment as TSqlScript).Batches[0].Statements[0] as SelectStatement).QueryExpression as QuerySpecification;

                    var select = GetSelect(query.SelectElements);
                    var where = GetWhere(query.WhereClause);
                    var order = GetOrder(query.OrderByClause);
                    return new SqlColumns(select, where, order);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        });



        private static void CheckUpError(string sql, IList<ParseError> errors)
        {
            if (errors.Count > 0)
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine(string.Format(Strings.NotCorrectSqlSelect, sql));
                foreach (var error in errors)
                {
                    message.AppendLine(error.Message);
                }
                throw new DataAccessException(message.ToString());
            }

        }

        private static IEnumerable<string> GetSelect(IList<SelectElement> elements)
        {
            return elements.Select((e) => GetSelectString(e)).Distinct();
        }


        private static  IEnumerable<string> GetWhere(WhereClause clause)
        {
            List<string> fields = new List<string>();
            if (clause == null) return fields;
            FillWhere(clause.SearchCondition, fields);
            return fields.Distinct();
        }



        private static void FillWhere(object target, List<string> fields)
        {
            var column = target as ColumnReferenceExpression;
            if (column != null)
            {
                fields.Add(GetString(column));
                return;
            }

            var func = target as FunctionCall;
            if (func != null)
            {
                fields.AddRange(GetFields(func));
                return;
            }

            var type = target.GetType();
            var firstExpression = GetExpression(type,target,"FirstExpression");
            if(firstExpression != null)
            {
                FillWhere(firstExpression, fields);
            }

            var secondExpression = GetExpression(type, target, "SecondExpression");
            if (secondExpression != null)
            {
                FillWhere(secondExpression, fields);
            }

            var expression = GetExpression(type, target, "Expression");
            if (expression != null)
            {
                FillWhere(expression, fields);
            }
        }

        private static object GetExpression(Type targetType, object target,string propertyName)
        {
            if (targetType.ResolveProperty(propertyName) == null) return null;
            return target.GetPropertyValue(propertyName);
        }


        private static IEnumerable<string> GetOrder(OrderByClause clause)
        {
            if (clause == null) return new List<string>();
            return clause.OrderByElements.Select((e) => GetString(e.Expression)).Distinct();
        }



        //private static SqlColumns GetColumnsByDecorator(ISqlNode node)
        //{
        //    var decorator = node as SelectDecorator;
        //    if (decorator != null)
        //    {
        //        var spec = decorator.SelectNode as SelectSpec;
        //        var selects = GetSelectColumns(spec);
        //        var wheres = GetWhereColumns(spec);
        //        var orders = GetOrerColumns(decorator.OrderBy);

        //        return new SqlColumns(selects, wheres, orders);
        //    }
        //    return null;
        //}

        //private static SqlColumns GetColumnsBySpec(ISqlNode node)
        //{
        //    var spec = node as SelectSpec;
        //    if (spec != null)
        //    {
        //        var selects = GetSelectColumns(spec);
        //        var wheres = GetWhereColumns(spec);
        //        return new SqlColumns(selects, wheres, new List<string>());
        //    }
        //    return null;
        //}

        //private static IEnumerable<string> GetSelectColumns(SelectSpec spec)
        //{
        //    return GetColumns(spec.Columns.AllTokens);

        //    //return spec.Columns.Select((t) =>
        //    //{
        //    //    return (t.ChildrenNodes.First() as SqlTokenIdentifier).Name;
        //    //});
        //}

        //private static IEnumerable<string> GetOrerColumns(SelectOrderBy sob)
        //{
        //    return GetColumns(sob.OrderByColumns.AllTokens);
        //    //return sob.OrderByColumns.Select((t) =>
        //    //{
        //    //    return (t.ChildrenNodes.First() as SqlTokenIdentifier).Name;
        //    //});
        //}

        //private static IEnumerable<string> GetWhereColumns(SelectSpec spec)
        //{
        //    List<string> columns = new List<string>();
        //    if (spec.WhereExpression == null) return columns;
        //    var operation = spec.WhereExpression as SqlBinaryOperator;

        //    return GetColumns(spec.WhereExpression.AllTokens);
        //}

        //private static IEnumerable<string> GetColumns(IEnumerable<SqlToken> tokens)
        //{
        //    return tokens.OfType<SqlTokenIdentifier>()
        //                                         .Where((token) => token.TokenType == SqlTokenType.IdentifierStandard)
        //                                         .Select((token) => token.Name).Distinct();
        //}

        private static string GetSelectString(TSqlFragment statement)
        {
            if (statement == null) return string.Empty;

            for (int i = statement.FirstTokenIndex; i <= statement.LastTokenIndex; i++)
            {
                //对于 name as xxx的形式，我们只取name
                var name = statement.ScriptTokenStream[i].Text;
                if (name.StartsWith("[")) 
                    return name.Substring(1, name.Length - 2);
                return name;
            }
            return string.Empty;
        }


        private static string GetString(TSqlFragment statement)
        {
            if (statement == null) return string.Empty;

            StringBuilder code = new StringBuilder();            

            for (int i = statement.FirstTokenIndex; i <= statement.LastTokenIndex; i++)
            {
                code.Append(statement.ScriptTokenStream[i].Text);
            }

            return code.ToString();
        }

        private static IEnumerable<string> GetFields(FunctionCall func)
        {
            List<string> fields = new List<string>();

            for (int i = func.FirstTokenIndex; i <= func.LastTokenIndex; i++)
            {
                var token = func.ScriptTokenStream[i];
                if(token.TokenType == TSqlTokenType.Identifier)
                {
                    fields.Add(token.Text);
                }
            }
            if (fields.Count == 0) return fields;

            var funcName = fields[0].ToLower();
            switch(funcName)
            {
                case "datediff":
                    {
                        var column = fields[2]; //该函数仅提取第3个参数作为列的名称
                        fields.Clear();
                        fields.Add(column);
                    }
                    break;
                default:
                    {
                        throw new DataAccessException(string.Format(Strings.UnrecognizedSqlFunction, funcName));
                    }
            }

            return fields;
        }


    }



}
