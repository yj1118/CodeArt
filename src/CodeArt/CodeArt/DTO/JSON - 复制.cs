using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.Concurrent;

namespace CodeArt.DTO
{
    public static class JSON
    {
        public static string WriteValue(object value, bool escape)
        {
            using (var temp = StringPool.Borrow())
            {
                var sb = temp.Item;
                WriteValue(sb, value, escape);
                return sb.ToString();
            }
        }

        public static void WriteValue(StringBuilder sb, object value, bool escape)
        {
            if (value == null || value == System.DBNull.Value)
            {
                sb.Append("null");
            }
            else if (value is string || value is Guid)
            {
                WriteString(sb, value.ToString(), escape);
            }
            else if (value is bool)
            {
                sb.Append(value.ToString().ToLower());
            }
            else if (value is double ||
                    value is float ||
                    value is long ||
                    value is int ||
                    value is short ||
                    value is byte ||
                    value is decimal)
            {
                sb.AppendFormat(CultureInfo.InvariantCulture.NumberFormat, "{0}", value);
            }
            else if (value.GetType().IsEnum)
            {
                sb.Append(System.Convert.ToInt32(value));
            }
            else if (value is DateTime)
            {
                sb.Append("new Date(\"");
                sb.Append(((DateTime)value).ToString("MMMM, d yyyy HH:mm:ss", new CultureInfo("en-US", false).DateTimeFormat));
                sb.Append("\")");
            }
            else if (value is Hashtable)
            {
                WriteHashtable(sb, value as Hashtable, escape);
            }
            else if (value is IDictionary)
            {
                WriteDictionary(sb, value as IDictionary, escape);
            }
            else if (value is IEnumerable)
            {
                WriteEnumerable(sb, value as IEnumerable, escape);
            }
            else
            {
                WriteObject(sb, value, escape);
            }
        }

        public static void WriteDictionary(StringBuilder sb, IDictionary value,bool escape)
        {
            sb.Append("{");
            foreach (DictionaryEntry item in value)
            {
                WriteValue(sb, item.Key, escape);
                sb.Append(":");
                WriteValue(sb, item.Value, escape);
                sb.Append(",");
            }
            if (value.Count > 0) --sb.Length;
            sb.Append("}");
        }

        public static void WriteString(StringBuilder sb, string value, bool escape)
        {
            if (escape)
            {
                WriteStringByEscape(sb, value);
            }
            else
            {
                sb.Append("\"");
                sb.Append(value);
                sb.Append("\"");
            }
        }

        private static void WriteStringByEscape(StringBuilder sb, string value)
        {
            //����Ҫ�ǽ������з���json���룬ÿ��ĩβ�����\�ַ�������js�������ʶ������е��ַ��� 
            //value = value.Replace("\\\r\n", "\r\n").Replace("\r\n", "\\n\\\r\n");
            //value = value.Replace("\r\n", "\\n\\\r\n");

            int pos = 0;
            sb.Append("\"");
            foreach (char c in value)
            {
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        {
                            //bool escape = false;
                            ////�����ң�����ҵ�"����ôת��
                            //for (var i = pos + 1; i < value.Length; i++)
                            //{
                            //    var temp = value[i];
                            //    if (temp == '\\') continue;
                            //    if (temp == '"')
                            //    {
                            //        escape = true;
                            //        break;
                            //    }
                            //    break;
                            //}
                            //if (escape) sb.Append("\\\\");
                            //else sb.Append(c);
                            sb.Append("\\\\");
                            break;
                        }
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        //int i = (int)c;
                        //if (i < 32 || i > 127)
                        //{
                        //    sb.AppendFormat("\\u{0:X04}", i);
                        //}
                        //else
                        //{
                        //    sb.Append(c);
                        //}
                        sb.Append(c);
                        break;
                }
                pos++;
            }
            sb.Append("\"");
        }

        private static void WriteObject(StringBuilder sb, object value,bool escape)
        {
            //���ݿɶ������Ժʹ���JSON��ǩ���ֶδ���json����
            IEnumerable<MemberInfo> members = value.GetType().GetPropertyAndFields();
            sb.Append("{");
            bool hasMembers = false;

            foreach (MemberInfo member in members)
            {
                string jsonName = member.Name;
                bool hasValue = false;
                object val = null;
                if ((member.MemberType & MemberTypes.Field) == MemberTypes.Field)
                {
                    FieldInfo field = (FieldInfo)member;
                    val = field.GetValue(value);
                    hasValue = true;
                }
                else if ((member.MemberType & MemberTypes.Property) == MemberTypes.Property)
                {
                    PropertyInfo property = (PropertyInfo)member;
                    if (property.CanRead && property.GetIndexParameters().Length == 0)
                    {
                        val = property.GetValue(value, null);
                        hasValue = true;
                    }
                }
                if (hasValue)
                {
                    sb.Append(jsonName == string.Empty ? member.Name : jsonName);
                    sb.Append(":");
                    WriteValue(sb, val, escape);
                    sb.Append(",");
                    hasMembers = true;
                }
            }
            if (hasMembers) --sb.Length;
            sb.Append("}");

        }

        private static void WriteHashtable(StringBuilder sb, Hashtable value,bool escape)
        {
            bool hasItems = false;
            sb.Append("{");
            foreach (string key in value.Keys)
            {
                sb.AppendFormat("\"{0}\":", key.ToLower());
                WriteValue(sb, value[key], escape);
                sb.Append(",");
                hasItems = true;
            }
            //�Ƴ����Ķ���
            if (hasItems) --sb.Length;
            sb.Append("}");
        }

        public static void WriteEnumerable(StringBuilder sb, IEnumerable value, bool escape)
        {
            bool hasItems = false;
            sb.Append("[");
            foreach (object val in value)
            {
                WriteValue(sb, val, escape);
                sb.Append(",");
                hasItems = true;
            }

            if (hasItems) --sb.Length;
            sb.Append("]");
        }


        /// <summary>
        /// �Ƿ�ת�壬������Ҫ��ָ�����з����հ׷��ȷ���ת��Ϊ������html��������ʾ�Ĵ���
        /// </summary>
        /// <param name="value"></param>
        /// <param name="escape"></param>
        /// <returns></returns>
        public static string GetCode(object value,bool escape)
        {
            using (var temp = StringPool.Borrow())
            {
                var sb = temp.Item;
                try
                {
                    WriteValue(sb, value, escape);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return sb.ToString();
            }
        }


        private readonly static RegexPool _datePool = new RegexPool("new Date\\(\"(.+?)\"\\)", RegexOptions.IgnoreCase);

        /// <summary>
        /// �����ַ��������ݣ��Զ�ʶ��Ϊ��Ӧ��object��ֵ
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static object GetValue(string code)
        {
            if (code == null || code == "null") return null;
            else if (code.EqualsIgnoreCase("true")) return true;
            else if (code.EqualsIgnoreCase("false")) return false;
            else
            {
                DateTime time = DateTime.MinValue;
                if (code.StartsWith("new Date"))
                {
                    using (var temp = _datePool.Borrow())
                    {
                        var reg = temp.Item;
                        var mc = reg.Match(code);
                        if (mc.Success) code = mc.Groups[1].Value;
                    }
                    if (DateTime.TryParse(code, out time)) return time;
                }


                int intValue = 0;
                if (int.TryParse(code, out intValue)) return intValue;

                long longValue = 0;
                if (long.TryParse(code, out longValue)) return longValue;

                float floatValue = 0;
                if (float.TryParse(code, out floatValue)) return floatValue;

                double doubleValue = 0;
                if (double.TryParse(code, out doubleValue)) return doubleValue;

                Guid guidValue = Guid.Empty;
                if (Guid.TryParse(code, out guidValue)) return guidValue;
            }
            return code;
        }

        /// <summary>
        /// ��ȡcode���ַ�����ֵ��Ҳ����˵ "123" ��ȡ����123����������,���һ��ᴦ��ת�������
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetStringValue(string code)
        {
            if (string.IsNullOrEmpty(code)) return string.Empty;
            //�����ŵĴ���
            if(code.Length >=2)
            {
                if (code[0] == '"' && code[code.Length - 1] == '"')
                {
                    code = code.Substring(1, code.Length - 2);
                    code = code.Replace("\\\"", "\"");
                }
                else if (code[0] == '\'' && code[code.Length - 1] == '\'')
                {
                    code = code.Substring(1, code.Length - 2);
                    code = code.Replace("\\\'", "\'");
                }
            }

            return code.Replace("\\b", "\b").Replace("\\f", "\f").Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\t", "\t").Replace("\\\\", "\\");
        }
    }
}
