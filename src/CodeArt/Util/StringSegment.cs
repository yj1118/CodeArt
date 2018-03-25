using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Util
{
    [DebuggerDisplay("{ToString()}")]
    public struct StringSegment
    {
        public string Source
        {
            get;
            private set;
        }

        public int Offset
        {
            get;
            private set;
        }

        public int Length
        {
            get;
            private set;
        }

        public StringSegment(string source, int offset, int length)
        {
            this.Source = source;
            this.Offset = offset;
            this.Length = length;
            _value = null;
        }

        public StringSegment(string source)
            : this(source, 0, source.Length)
        {
        }

        #region Trim

        /// <summary>
        /// 返回从当前 StringSegment 对象移除所有前导空白字符和尾部空白字符的副本
        /// </summary>
        /// <returns></returns>
        public StringSegment Trim()
        {
            return TrimHelper(2);
        }

        public StringSegment TrimStart()
        {
            return TrimHelper(0);
        }

        public StringSegment TrimEnd()
        {
            return TrimHelper(1);
        }


        private StringSegment TrimHelper(int trimType)
        {
            if (this.IsNull()) return StringSegment.Empty;

            int length = this.Offset + this.Length;
            int end = length - 1;
            int start = this.Offset;
            if (trimType != 1)
            {
                //移除前导空白
                start = this.Offset;
                while (start < length)
                {
                    if (!char.IsWhiteSpace(_GetChar(start)))
                    {
                        break;
                    }
                    start++;
                }
            }
            if (trimType != 0)
            {
                //移除后置空白
                end = length - 1;
                while (end >= start)
                {
                    if (!char.IsWhiteSpace(_GetChar(end)))
                    {
                        break;
                    }
                    end--;
                }
            }
            return CreateTrimmedSegment(start, end);
        }

        private string CreateTrimmedSegment(int start, int end)
        {
            int length = (end - start) + 1;
            if (length == this.Length)
            {
                return this;
            }
            if (length == 0)
            {
                return Empty;
            }
            return new StringSegment(this.Source, start, length);
        }

        #endregion

        #region StartsWith

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase">忽略大小写</param>
        /// <returns></returns>
        public bool StartsWith(string value, bool ignoreCase)
        {
            if (this.IsNull()) return false;

            if (value.Length == 0)
            {
                return true;
            }

            if (value.Length > this.Length)
            {
                return false;
            }

            var equals = GetEquals(ignoreCase);

            int pointer = 0;
            var length = this.Offset + this.Length;
            for (var i = this.Offset; i < length; i++)
            {
                var c = _GetChar(i);
                if (!equals(c, value[pointer])) return false;
                pointer++;
                if (pointer == value.Length) return true; //对比完了
            }
            return false;
        }

        public bool StartsWith(string value)
        {
            return StartsWith(value, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase">忽略大小写</param>
        /// <returns></returns>
        public bool StartsWith(char value, bool ignoreCase)
        {
            if (this.IsNull()) return false;

            if (this.Length == 0)
            {
                return false;
            }

            var equals = GetEquals(ignoreCase);
            var firstChar = GetFirstChar();
            return equals(firstChar, value);
        }

        public bool StartsWith(char value)
        {
            return StartsWith(value, false);
        }

        #endregion

        #region EndsWith

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase">忽略大小写</param>
        /// <returns></returns>
        public bool EndsWith(string value, bool ignoreCase)
        {
            if (this.IsNull()) return false;

            if (value.Length == 0)
            {
                return true;
            }

            if (value.Length > this.Length)
            {
                return false;
            }

            var equals = GetEquals(ignoreCase);

            int length = this.Offset + this.Length;
            int pointer = value.Length - 1;
            for (var i = length - 1; i >= this.Offset; i--)
            {
                var c = _GetChar(i);
                if (pointer < 0) break;
                if (!equals(c, value[pointer])) return false;
                pointer--;
            }

            return true;
        }

        public bool EndsWith(string value)
        {
            return EndsWith(value, false);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase">忽略大小写</param>
        /// <returns></returns>
        public bool EndsWith(char value, bool ignoreCase)
        {
            if (this.IsNull()) return false;

            if (this.Length == 0)
            {
                return false;
            }

            var equals = GetEquals(ignoreCase);
            var lastChar = GetLastChar();
            return equals(lastChar, value);
        }

        public bool EndsWith(char value)
        {
            return EndsWith(value, false);
        }


        #endregion

        #region IndexOf

        public int IndexOf(char value, bool ignoreCase)
        {
            if (this.IsNull()) return -1;
            if (this.Length == 0) return -1;

            var equals = GetEquals(ignoreCase);

            int length = this.Offset + this.Length;

            for (var i = this.Offset; i < length; i++)
            {
                var c = _GetChar(i);
                if (equals(c, value)) return i - this.Offset; //返回的是相对偏移量
            }
            return -1;
        }

        public int IndexOf(char value)
        {
            return IndexOf(value, false);
        }


        public int IndexOf(string value, bool ignoreCase)
        {
            if (this.IsNull()) return -1;
            if (this.Length == 0) return -1;

            var equals = GetEquals(ignoreCase);

            for (var i = 0; i < this.Length; i++)
            {
                var length = this.Length - i;
                if (length < value.Length) return -1;
                var position = IndexOf(i, value, equals);
                if (position > -1) return position;
            }
            return -1;
        }

        private int IndexOf(int startIndex, string value, Func<char, char, bool> equals)
        {
            var count = this.Length - startIndex;
            if (count < value.Length) return -1;

            var offset = this.Offset + startIndex;
            var length = offset + count;

            int pointer = 0;
            for (var i = offset; i < length; i++)
            {
                var c = _GetChar(i);
                if (!equals(c, value[pointer])) return -1;
                pointer++;
                if (pointer == value.Length) return i - value.Length + 1 - this.Offset; //返回的是相对偏移量
            }
            return -1;
        }


        public int IndexOf(string value)
        {
            return IndexOf(value, false);
        }

        #endregion

        #region Substring

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public StringSegment Substring(int startIndex, int length)
        {
            if (this.IsNull()) return StringSegment.Empty;

            if (startIndex < 0 || startIndex > this.Length)
            {
                throw new ArgumentOutOfRangeException("startIndex", string.Format(Strings.ArgumentOutOfRange, "startIndex"));
            }

            if (length < 0 || (startIndex > (this.Length - length)))
            {
                throw new ArgumentOutOfRangeException("length", string.Format(Strings.ArgumentOutOfRange, "length"));
            }

            if (length == 0)
            {
                return Empty;
            }

            if ((startIndex == 0) && (length == this.Length))
            {
                return this;
            }

            var offset = this.Offset + startIndex;
            return new StringSegment(this.Source, offset, length);
        }

        public StringSegment Substring(int startIndex)
        {
            return this.Substring(startIndex, this.Length - startIndex);
        }


        #endregion

        #region 辅助

        public char GetFirstChar()
        {
            return _GetChar(this.Offset);
        }

        public char GetLastChar()
        {
            return _GetChar(this.Offset + this.Length - 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">相对下标</param>
        /// <returns></returns>
        public char GetChar(int index)
        {
            return _GetChar(this.Offset + index);
        }

        /// <summary>
        /// 内部的_GetChar方法接受的参数是绝对下b标
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private char _GetChar(int index)
        {
            return this.Source[index];
        }

        private Func<char, char, bool> GetEquals(bool ignoreCase)
        {
            return ignoreCase ? CharIgnoreCaseEquals : CharEquals;
        }

        private static Func<char, char, bool> CharIgnoreCaseEquals = new Func<char, char, bool>((a, b) =>
        {
            return a.IgnoreCaseEquals(b);
        });

        private static Func<char, char, bool> CharEquals = new Func<char, char, bool>((a, b) =>
        {
            return a == b;
        });

        #endregion

        private string _value;

        public override string ToString()
        {
            if (_value == null)
            {
                if (this.IsNull()) return null;
                _value = this.Source.Substring(this.Offset, this.Length);
            }
            return _value;
        }

        public bool IsNull()
        {
            return this.Source == null;
        }

        public static readonly StringSegment Null = new StringSegment(null, 0, 0);

        public bool IsEmpty()
        {
            return this.Offset == 0 && this.Length == 0;
        }



        public static readonly StringSegment Empty = new StringSegment("empty", 0, 0);

        public static implicit operator StringSegment(string value)
        {
            return new StringSegment(value);
        }

        public static implicit operator string(StringSegment segment)
        {
            return segment.ToString();
        }

    }
}
