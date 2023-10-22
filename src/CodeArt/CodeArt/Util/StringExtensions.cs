using System;
using System.Linq;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Threading;
using CodeArt.Log;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace CodeArt.Util
{
    /// <summary>
    /// 字符串扩展
    /// </summary>
    public static class StringExtensions
    {
        #region 字符串

        /// <summary>
        /// 忽略大小写的比较
        /// </summary>
        /// <param name="str"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string str, string value)
        {
            if (str == null) return value == null;
            return str.Equals(value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// <para>高效的判定在字符串中{startIndex,length}区域的内容，是否与value匹配</para>
        /// <para>用此方法，不需要再内存中开辟新的字符串</para>
        /// <para>待测试</para>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startIndex"></param>
        /// <param name="value"></param>
        /// <param name="ignoreCase">true:忽略大小写比对，false:区分大小写</param>
        /// <returns></returns>
        public static bool EqualsWith(this string str, int startIndex, string value, bool ignoreCase)
        {
            if ((startIndex + value.Length) > str.Length) return false;
            if (ignoreCase)
            {
                for (var i = 0; i < value.Length; i++)
                {
                    if (!str[startIndex + i].IgnoreCaseEquals(value[i])) return false;
                }
            }
            else
            {
                for (var i = 0; i < value.Length; i++)
                {
                    if (str[startIndex + i] != value[i]) return false;
                }
            }
            return true;
        }

        public static bool EqualsWith(this string str, int startIndex, string value)
        {
            return EqualsWith(str, startIndex, value, false);
        }

        #endregion

        #region 大小写转换

        public static string FirstToUpper(this string str)
        {
            return str.ToUpper(0, 1);
        }

        public static string ToUpper(this string str, int startIndex, int length)
        {
            return ChangeLowerOrUpper(str, startIndex, length, (i, len) =>
            {
                return str.Substring(i, len).ToUpper();
            });
        }

        public static string FirstToLower(this string str)
        {
            return str.ToLower(0, 1);
        }

        public static string ToLower(this string str, int startIndex, int length)
        {
            return ChangeLowerOrUpper(str, startIndex, length, (i, len) =>
            {
                return str.Substring(i, len).ToLower();
            });
        }

        private static string ChangeLowerOrUpper(string str, int startIndex, int length, Func<int, int, string> transform)
        {
            if (str.Length == 0) return string.Empty;
            if (startIndex == 0)
            {
                length = length > str.Length ? str.Length : length;
                return string.Format("{0}{1}", transform(0, length),
                                               str.Substring(length));
            }
            else
            {
                int lastIndex = str.Length - 1;
                if (startIndex >= lastIndex) return str;

                int rightLength = str.Length - startIndex - length;
                if (rightLength < 0)
                    length += rightLength;
                return string.Format("{0}{1}{2}", str.Substring(0, startIndex),
                                                transform(startIndex, length),
                                                str.Substring(startIndex + length));
            }
        }

        public static string TrimStart(this string str, string trimValue)
        {
            while (str.IndexOf(trimValue) == 0)
            {
                str = str.Substring(trimValue.Length);
            }
            return str;
        }

        public static string TrimEnd(this string str, string trimValue)
        {
            int p = str.LastIndexOf(trimValue);

            while (p != -1 && p == (str.Length - trimValue.Length))
            {
                str = str.Substring(0, p);
                p = str.LastIndexOf(trimValue);
            }
            return str;
        }

        public static string Trim(this string str, string trimValue)
        {
            str = str.TrimStart(trimValue);
            return str.TrimEnd(trimValue);
        }

        public static string Substr(this string str, int startIndex, int count, string suffix)
        {
            if (startIndex == 0 && str.Length <= count) return str;
            var length = count - startIndex;
            var actualLength = str.Length - startIndex;
            return length >= actualLength ? str.Substring(startIndex, actualLength) : string.Format("{0}{1}", str.Substring(startIndex, length), suffix);
        }

        public static string Substr(this string str, int startIndex, int count)
        {
            return str.Substr(startIndex, count, string.Empty);
        }

        #region Base64编码

        public static string ToBase64(this string str)
        {
            return ToBase64(str, Encoding.UTF8);
        }

        public static string ToBase64(this string str, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }

        public static string FromBase64(this string str)
        {
            return FromBase64(str, Encoding.UTF8);
        }

        public static string FromBase64(this string str, Encoding encoding)
        {
            byte[] bytes = Convert.FromBase64String(str);
            return encoding.GetString(bytes);
        }

        #endregion


        public static string SHA1(this string str)
        {
            byte[] temp = null;
            using (SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider())
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(str);
                temp = sha.ComputeHash(dataBytes);
            }

            string output = BitConverter.ToString(temp);
            output = output.Replace("-", "");
            output = output.ToLower();
            return output;
        }

        public static string MD5(this string str)
        {
            MD5 md = new MD5CryptoServiceProvider();
            return BitConverter.ToString(md.ComputeHash(new UTF8Encoding().GetBytes(str)));
        }


        /// <summary>
        /// 和BCrypt类似的PBKDF2，PBKDF2同样也可以通过参数设定重复计算的次数从而延长计算时间，防止暴力词典破解。
        /// </summary>
        /// <param name="str"></param>
        /// <param name="salt">salt大小必须是8个字节或更大</param>
        /// <param name="length">获取编码后的字符串的长度</param>
        /// <returns></returns>
        public static string PBKDF2(this string str, byte[] salt, int length)
        {
            var kd = new Rfc2898DeriveBytes(str, salt);//salt 大小必须是 8 个字节或更大和迭代次数必须是大于零。 建议的最小迭代数为 1000,迭代次数越多，耗费性能越大，这样别人就无法暴力破解
            return BitConverter.ToString(kd.GetBytes(length)).Replace("-", "").ToLower();
        }

        /// <summary>  
        /// 判断是否为汉字  
        /// </summary>  
        /// <param   name="value">待检测字符串</param>  
        /// <returns>是汉字返回true</returns>  
        public static bool IsChinese(this string value)
        {
            return Regex.IsMatch(value, "[\u4e00-\u9fa5]");
        }

        public static bool IsASCII(this string value)
        {
            return Encoding.UTF8.GetByteCount(value) == value.Length;
        }

        /// <summary>
        /// 得到每个汉字的字首拼音码字母(大写)
        /// </summary>
        /// <param name="chrStr">输入字符串</param>
        /// <returns>返回结果</returns>
        private static string GetHeadCharacters(string chrStr, int count)
        {
            string strHeadString = string.Empty;

            Encoding gb = System.Text.Encoding.GetEncoding("gb2312");
            for (int i = 0; i < chrStr.Length; i++)
            {
                if (i >= count) break;
                var current = chrStr.Substring(i, 1);
                //检测该字符是否为汉字
                if (!IsChinese(current))
                {
                    strHeadString += MapNumber(current);
                    continue;
                }

                byte[] bytes = gb.GetBytes(current);
                string lowCode = System.Convert.ToString(bytes[0] - 0xA0, 16);
                string hightCode = System.Convert.ToString(bytes[1] - 0xA0, 16);
                int nCode = Convert.ToUInt16(lowCode, 16) * 100 + Convert.ToUInt16(hightCode, 16);      //得到区位码
                strHeadString += FirstLetter(nCode);
            }
            return strHeadString;
        }

        private static string MapNumber(string value)
        {
            switch (value)
            {
                case "1": return "E";
                case "2": return "E";
                case "3": return "S";
                case "4": return "S";
                case "5": return "W";
                case "6": return "L";
                case "7": return "Q";
                case "8": return "B";
                case "9": return "J";
            }
            return value;
        }

        /// <summary>
        /// 通过汉字区位码得到其首字母(大写)
        /// </summary>
        /// <param name="nCode">汉字编码</param>
        /// <returns></returns>
        private static string FirstLetter(int nCode)
        {
            if (nCode >= 1601 && nCode < 1637) return "A";
            if (nCode >= 1637 && nCode < 1833) return "B";
            if (nCode >= 1833 && nCode < 2078) return "C";
            if (nCode >= 2078 && nCode < 2274) return "D";
            if (nCode >= 2274 && nCode < 2302) return "E";
            if (nCode >= 2302 && nCode < 2433) return "F";
            if (nCode >= 2433 && nCode < 2594) return "G";
            if (nCode >= 2594 && nCode < 2787) return "H";
            if (nCode >= 2787 && nCode < 3106) return "J";
            if (nCode >= 3106 && nCode < 3212) return "K";
            if (nCode >= 3212 && nCode < 3472) return "L";
            if (nCode >= 3472 && nCode < 3635) return "M";
            if (nCode >= 3635 && nCode < 3722) return "N";
            if (nCode >= 3722 && nCode < 3730) return "O";
            if (nCode >= 3730 && nCode < 3858) return "P";
            if (nCode >= 3858 && nCode < 4027) return "Q";
            if (nCode >= 4027 && nCode < 4086) return "R";
            if (nCode >= 4086 && nCode < 4390) return "S";
            if (nCode >= 4390 && nCode < 4558) return "T";
            if (nCode >= 4558 && nCode < 4684) return "W";
            if (nCode >= 4684 && nCode < 4925) return "X";
            if (nCode >= 4925 && nCode < 5249) return "Y";
            if (nCode >= 5249 && nCode < 5590) return "Z";
            return string.Empty;
        }

        /// <summary>
        /// 得到一个汉字的拼音第一个字母，如果是一个英文字母则直接返回大写字母 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetFirstLetter(this string str)
        {
            return GetHeadCharacters(str, 1);
        }

        #endregion

    }
}
